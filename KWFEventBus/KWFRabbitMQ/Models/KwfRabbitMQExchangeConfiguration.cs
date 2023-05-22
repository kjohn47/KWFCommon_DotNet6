namespace KWFEventBus.KWFRabbitMQ.Models
{
    using KWFEventBus.Abstractions.Models;

    public class KwfRabbitMQExchangeConfiguration
    {
        private static KwfRabbitMQExchangeConfiguration _defaultInstance = new KwfRabbitMQExchangeConfiguration
        {
            ExchangeName = "kwf.exchange",
            Durable = true,
            AutoDelete = false,
            Type = ExchangeTypeEnum.Direct,
            Arguments = null
        };

        public string ExchangeName { get; set; } = string.Empty;
        public bool? Durable { get; set; }
        public bool? AutoDelete { get; set; }
        public ExchangeTypeEnum? Type { get; set; }
        public IEnumerable<EventBusProperty>? Arguments { get; set; }

        public static KwfRabbitMQExchangeConfiguration GetDefaultInsance() 
        {
            return _defaultInstance;
        }

        public IDictionary<string, object>? GetArguments()
        {
            if (Arguments == null)
            {
                return null;
            }

            return Arguments.ToDictionary(a => a.PropertyName, a => (object)a.PropertyValue);
        }
    }
}
