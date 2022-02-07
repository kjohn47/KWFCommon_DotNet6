namespace KWFEventBus.Abstractions.Interfaces
{
    using System.Threading.Tasks;

    public interface IKwfEventHandler<TPayload> where TPayload : class
    {
        Task HandleEventAsync(IEventPayloadEnvelope<TPayload> eventData);
    }
}
