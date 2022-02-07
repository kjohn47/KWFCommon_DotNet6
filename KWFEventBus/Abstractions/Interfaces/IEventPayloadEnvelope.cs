namespace KWFEventBus.Abstractions.Interfaces
{
    public interface IEventPayloadEnvelope<T> where T : class
    {
        T Payload { get; }
        Guid Id { get; }
        DateTime TimeStamp { get; }
    }
}
