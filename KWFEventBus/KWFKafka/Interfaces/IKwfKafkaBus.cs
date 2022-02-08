namespace KWFEventBus.KWFKafka.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaBus : IKWFEventBusProducer
    {
        IKwfEventConsumerHandler CreateConsumer<TPayload>(
            IKwfEventHandler<TPayload> eventHandler,
            string topic,
            string? topipConfigurationKey = null)
        where TPayload : class;
    }
}
