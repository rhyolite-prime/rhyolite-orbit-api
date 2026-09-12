using System;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhyoliteOrbit.RabbitMq;

namespace RhyoliteOrbit.Actors;

public class DataSupportEntitySyncActor: ReceiveActor
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly string _emailNotificationQueue;
    private readonly string _topicExchangeType;
    
    public DataSupportEntitySyncActor(IServiceProvider serviceProvider, IConfiguration configuration)   
    {
        _rabbitMqClient = serviceProvider.GetRequiredService<IRabbitMqClient>();
        _emailNotificationQueue = configuration["RabbitMqBroker:DataSupportEntitySyncQueue"];
        _topicExchangeType = configuration["RabbitMqBroker:ExchangeTypes:AmqTopic"];
        
        ReceiveAsync<object>(async a => await HandleMessage(a));
    }
    
    private async Task HandleMessage(object msg)
    {
        await Task.Delay(2);
        
        _rabbitMqClient.Produce(_topicExchangeType, _emailNotificationQueue, msg);
        
    }
    
}