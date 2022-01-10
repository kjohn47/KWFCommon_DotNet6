namespace KWFEventBus.Abstractions
{
    public interface IEventBusProperty
    {
        string PropertyName { get; }
        string PropertyValue { get; }
    }
}
