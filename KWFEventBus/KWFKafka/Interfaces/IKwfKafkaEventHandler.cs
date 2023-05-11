namespace KWFEventBus.KWFKafka.Interfaces
{
    using KWFEventBus.Abstractions.Interfaces;

    public interface IKwfKafkaEventHandler<TPayload> : IKwfEventHandler<TPayload> where TPayload : class
    {
    }
}
