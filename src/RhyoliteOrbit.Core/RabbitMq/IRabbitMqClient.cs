namespace RhyoliteOrbit.RabbitMq;

public interface IRabbitMqClient: Abp.Domain.Services.IDomainService
{
    void Produce(string exchangeName, string queueName, object message);
}