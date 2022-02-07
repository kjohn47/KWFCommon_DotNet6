namespace KWFEventBus.KWFKafka.Interfaces
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaBus : IKWFEventBusProducer
    {
        Task<IConsumer<string, byte[]>> RegisterConsumer<TPayload>(IKwfEventHandler<TPayload> handler, string topic, string? topipConfigurationKey = null)
            where TPayload : class;
    }
}
