namespace KWFEventBus.Abstractions
{
    public interface IKWFEventBusProducer
    {
        void ProduceAsync<T>(T payload, string topic) where T : class;

        void ProduceAsync<T>(T payload, string topic, IKWFEventBusConfiguration configuration) where T : class;
    }
}
