namespace KWFEventBus.KWFKafka.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaBus : IKWFEventBusProducer
    {
        IKwfKafkaEventConsumerHandler CreateConsumer<THandler, TPayload>(
            THandler eventHandler,
            string topic,
            string? topipConfigurationKey = null)
        where THandler : class, IKwfEventHandler<TPayload>
        where TPayload : class;
    }
}
