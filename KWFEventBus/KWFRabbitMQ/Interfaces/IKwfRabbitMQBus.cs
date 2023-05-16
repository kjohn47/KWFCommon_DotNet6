namespace KWFEventBus.KWFRabbitMQ.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfRabbitMQBus : IKWFEventBusProducer
    {
        IKwfRabbitMQConsumerHandler CreateConsumer<THandler, TPayload>(
            THandler eventHandler,
            string topic,
            string? topipConfigurationKey = null)
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class;
    }
}
