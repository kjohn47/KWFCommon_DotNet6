namespace KWFEventBus.Abstractions.Interfaces
{
    using KWFEventBus.Abstractions.Models;

    public interface IKWFEventBusConfiguration
    {
        string AppName { get; }
        IEnumerable<EventBusEndpoint>? Endpoints { get; }
        IEnumerable<EventBusProperty>? CommonProperties { get; }
        IEnumerable<EventBusProperty>? ProducerProperties { get; }
        IEnumerable<EventBusProperty>? ConsumerProperties { get; }
    }
}
