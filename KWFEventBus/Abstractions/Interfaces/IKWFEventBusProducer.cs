namespace KWFEventBus.Abstractions.Interfaces
{
    public interface IKWFEventBusProducer
    {
        Task ProduceAsync<T>(T payload, string topic, string? key, CancellationToken? cancellationToken = null) where T : class;

        Task ProduceAsync<T>(T payload, string topic, CancellationToken? cancellationToken = null) where T : class;
    }
}
