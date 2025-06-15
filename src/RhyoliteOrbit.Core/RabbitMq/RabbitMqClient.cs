using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace RhyoliteOrbit.RabbitMq;

public class RabbitMqClient: Abp.Domain.Services.DomainService, IRabbitMqClient
{
    private readonly string _uri;

    public RabbitMqClient(IConfiguration configuration)
    {
        _uri = configuration["RabbitMqBroker:Url"];

    }
    public void Produce(string exchangeName, string queueName, object message)
    {
        var factory = new ConnectionFactory() { Uri = new Uri(_uri) };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

        channel.BasicPublish(exchange: exchangeName,
            routingKey: queueName,
            basicProperties: null,
            body: body);

        Logger.Info($"published => {JsonConvert.SerializeObject(message)}");
    }
}