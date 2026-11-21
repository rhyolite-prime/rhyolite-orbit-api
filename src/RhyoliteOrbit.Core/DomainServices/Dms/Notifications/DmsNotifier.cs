using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RhyoliteOrbit.Models.Dms;
using RhyoliteOrbit.Notifications;
using RhyoliteOrbit.RabbitMq;

namespace RhyoliteOrbit.DomainServices.Dms.Notifications;

/// <summary>
/// Publishes document management domain events. RabbitMQ publishing is best effort:
/// when no broker is configured the event is only logged.
/// </summary>
public interface IDmsNotifier
{
    Task PublishAsync(string eventType, Guid aggregateId, object payload);

    Task SendEmailAsync(string to, string subject, string body);
}

public class DmsNotifier: IDmsNotifier, ITransientDependency
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly ICustomEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly IRepository<DmsDocumentEvent, Guid> _eventRepository;
    private readonly IAbpSession _abpSession;

    public DmsNotifier(
        IRabbitMqClient rabbitMqClient,
        ICustomEmailSender emailSender,
        IConfiguration configuration,
        IRepository<DmsDocumentEvent, Guid> eventRepository,
        IAbpSession abpSession)
    {
        _rabbitMqClient = rabbitMqClient;
        _emailSender = emailSender;
        _configuration = configuration;
        _eventRepository = eventRepository;
        _abpSession = abpSession;
    }

    public async Task PublishAsync(string eventType, Guid aggregateId, object payload)
    {
        var payloadJson = payload == null ? null : JsonConvert.SerializeObject(payload);

        // Transactional outbox: the event survives even when the broker is down.
        await _eventRepository.InsertAsync(new DmsDocumentEvent
        {
            TenantId = _abpSession.TenantId ?? 0,
            EventType = eventType,
            AggregateId = aggregateId,
            PayloadJson = payloadJson,
            OccurredAt = DateTime.UtcNow
        });

        var exchange = _configuration["RabbitMqBroker:ExchangeNames:DmsEvents"] ?? "dms.events";
        var queue = _configuration["RabbitMqBroker:DmsEventsQueue"] ?? "dms.events.queue";

        var message = new
        {
            EventType = eventType,
            AggregateId = aggregateId,
            OccurredAt = DateTime.UtcNow,
            Payload = payload
        };

        try
        {
            _rabbitMqClient.Produce(exchange, queue, message);
        }
        catch (Exception)
        {
            // The broker is optional; the outbox row keeps the event for a retry.
        }

        await Task.CompletedTask;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            return;
        }

        try
        {
            await _emailSender.SendEmailAsync(to, subject, body);
        }
        catch (Exception)
        {
            // notifications must never break the business operation
        }
    }
}
