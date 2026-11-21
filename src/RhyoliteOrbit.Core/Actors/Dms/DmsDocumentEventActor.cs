using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RhyoliteOrbit.RabbitMq;

namespace RhyoliteOrbit.Actors.Dms;

/// <summary>
/// A document lifecycle event emitted by the DMS (upload, version, share,
/// workflow, retention, ...). Carried through the actor system and forwarded to
/// the message broker so other modules can react without a direct dependency.
/// </summary>
public class DmsDocumentEventMessage
{
    public string EventType { get; set; }

    public Guid AggregateId { get; set; }

    public string PayloadJson { get; set; }

    public int TenantId { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Forwards document events to RabbitMQ. The broker is optional: when it is not
/// configured the message is dropped (and logged) so a DMS deployment without a
/// broker keeps working.
/// </summary>
public class DmsDocumentEventActor: ReceiveActor
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly string _documentEventQueue;
    private readonly string _topicExchangeType;
    private readonly ILogger<DmsDocumentEventActor> _logger;

    public DmsDocumentEventActor(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _rabbitMqClient = serviceProvider.GetService<IRabbitMqClient>();
        _documentEventQueue = configuration["RabbitMqBroker:DocumentEventQueue"] ?? "dms.document.events";
        _topicExchangeType = configuration["RabbitMqBroker:ExchangeTypes:AmqTopic"] ?? "topic";
        _logger = serviceProvider.GetService<ILogger<DmsDocumentEventActor>>();

        ReceiveAsync<DmsDocumentEventMessage>(async message => await HandleMessageAsync(message));
    }

    private async Task HandleMessageAsync(DmsDocumentEventMessage message)
    {
        if (_rabbitMqClient == null)
        {
            _logger?.LogWarning("RabbitMQ is not configured; dropping document event {EventType}.", message.EventType);
            await Task.CompletedTask;
            return;
        }

        try
        {
            _rabbitMqClient.Produce(_topicExchangeType, _documentEventQueue, message);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Could not publish document event {EventType}.", message.EventType);
        }

        await Task.CompletedTask;
    }
}
