namespace KWFEventBus.KWFKafka.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaBus : IKWFEventBusProducer
    {
        Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class;

        IKwfKafkaEventConsumerHandler CreateConsumer<THandler, TPayload>(
            THandler eventHandler,
            string topic,
            string? topipConfigurationKey = null)
        where THandler : class, IKwfKafkaEventHandler<TPayload>
        where TPayload : class;
    }
}
