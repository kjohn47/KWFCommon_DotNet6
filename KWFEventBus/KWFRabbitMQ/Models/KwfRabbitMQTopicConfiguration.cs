namespace KWFEventBus.KWFRabbitMQ.Models
{
    using KWFEventBus.Abstractions.Models;

    public class KwfRabbitMQTopicConfiguration
    {
        public bool? AutoQueueCreation { get; set; }
        public bool? Durable { get; set; }
        public bool? AutoDelete { get; set; }
        public bool? Exclusive { get; set; }
        public bool? WaitAck { get; set; }
        public bool? MessagePersistent { get; set; }
        public bool? AutoCommit { get; set; }
        public bool? RequeueOnFail { get; set; }
        public bool? EnableDlq { get; set; }
        public KwfRabbitMQExchangeConfiguration? ExchangeConfiguration { get; set; }
        public IEnumerable<EventBusProperty>? Headers { get; set; }
        public IEnumerable<EventBusProperty>? Arguments { get; set; }
    }
}
