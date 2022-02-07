namespace KWFEventBus.KWFKafka.Interfaces
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaBus : IKWFEventBusProducer
    {
        (IConsumer<string, byte[]>, ConsumerConfig) CreateConsumer(string? topipConfigurationKey = null);
    }
}
