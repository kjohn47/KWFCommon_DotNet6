namespace KWFEventBus.KWFRabbitMQ.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfRabbitMQBus : IKWFEventBusProducer
    {
        Task ProduceAsync<T>(T payload, string topic, string? configurationKey, CancellationToken? cancellationToken = null) where T : class;

        IKwfRabbitMQConsumerHandler CreateConsumer<THandler, TPayload>(
            THandler eventHandler,
            string topic,
            string? configurationKey = null)
        where THandler : class, IKwfRabbitMQEventHandler<TPayload>
        where TPayload : class;
    }
}
