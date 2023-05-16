namespace KWFEventBus.KWFRabbitMQ.Models
{
    using System.Collections.Generic;


    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;

    public class KwfRabbitMQConfiguration : IKWFEventBusConfiguration
    {
        private string? _endpoints;

        public string AppName { get; set; } = string.Empty;
        public string ClientName { get; set; } = Environment.MachineName;
        public int ConsumerTimeout { get; set; } = 5000;
        public int ConsumerMaxRetries { get; set; } = 5;
        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }
        public IEnumerable<EventBusProperty>? CommonProperties { get; set; }
        public IEnumerable<EventBusProperty>? ProducerProperties { get; set; }
        public IEnumerable<EventBusProperty>? ConsumerProperties { get; set; }
    }
}
