namespace KWFEventBus.Abstractions
{
    public interface ITopicProperties
    {
        IEnumerable<string> Topics { get; }
        IEnumerable<IEventBusProperty>? CommonProperties { get; }
        IEnumerable<IEventBusProperty>? ProducerProperties { get; }
        IEnumerable<IEventBusProperty>? ConsumerProperties { get; }
    }
}
