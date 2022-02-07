namespace KWFEventBus.KWFKafka.Interfaces
{
    public interface IKwfEventConsumerHandler
    {
        void StartConsuming();
        void StopConsuming();
    }
}
