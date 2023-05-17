namespace KWFEventBus.Abstractions.Interfaces
{
    using KWFEventBus.Abstractions.Models;

    public interface IKWFEventBusConfiguration
    {
        string AppName { get; }
        IEnumerable<EventBusEndpoint>? Endpoints { get; }
    }
}
