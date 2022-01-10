namespace KWFEventBus.Abstractions
{
    public interface IKWFEventBusConfiguration
    {
        string AppName { get; }
        string Url { get; }
        IEnumerable<IEventBusProperty>? CommonProperties { get; }
        IEnumerable<IEventBusProperty>? ProducerProperties { get; }
        IEnumerable<IEventBusProperty>? ConsumerProperties { get; }
        IEnumerable<ITopicProperties>? TopicProperties { get; }
    }
}
