namespace KWFEventBus.Abstractions.Interfaces
{
    public interface IKWFEventBusProducer
    {
        Task ProduceAsync<T>(T payload, string topic, CancellationToken? cancellationToken = null) where T : class;
        Task ProduceMultipleAsync<T>(T payload, string[] topics, CancellationToken? cancellationToken = null) where T : class;
    }
}
