using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhyoliteOrbit.Actors.MessageModels;
using RhyoliteOrbit.RabbitMq;

namespace RhyoliteOrbit.Actors;

public class EmailActor: ReceiveActor
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly string _emailNotificationQueue;
    private readonly string _topicExchangeType;
    public EmailActor(IServiceProvider serviceProvider, IConfiguration configuration)   
    {
        _rabbitMqClient = serviceProvider.GetRequiredService<IRabbitMqClient>();
        _emailNotificationQueue = configuration["RabbitMqBroker:EmailNotificationQueue"];
        _topicExchangeType = configuration["RabbitMqBroker:ExchangeTypes:AmqTopic"];
        
        ReceiveAsync<EmailNotification>(async a => await HandleMessage(a));
    }
    
    private async Task HandleMessage(EmailNotification msg)
    {
        await Task.Delay(1);
        
        _rabbitMqClient.Produce(_topicExchangeType, _emailNotificationQueue, msg);
        
    }
}