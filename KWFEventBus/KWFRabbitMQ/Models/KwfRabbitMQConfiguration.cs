namespace KWFEventBus.KWFRabbitMQ.Models
{
    using System.Collections.Generic;


    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;

    public class KwfRabbitMQConfiguration : IKWFEventBusConfiguration
    {
        public string AppName { get; set; } = "KWFApp";
        public string ClientName { get; set; } = Environment.MachineName;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string ExchangeName { get; set; } = string.Empty;
        public int ConsumerTimeout { get; set; } = 5000;
        public int ProducerTimeout { get; set; } = 1000;
        public int ConsumerMaxRetries { get; set; } = 5;
        public bool AutoQueueCreation { get; set; } = true;
        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }

        public string GetClientName()
        {
            return $"{ClientName}.{AppName}";
        }
    }
}
