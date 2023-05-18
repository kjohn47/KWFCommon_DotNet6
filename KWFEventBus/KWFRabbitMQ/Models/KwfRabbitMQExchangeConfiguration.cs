namespace KWFEventBus.KWFRabbitMQ.Models
{
    public class KwfRabbitMQExchangeConfiguration
    {
        private static KwfRabbitMQExchangeConfiguration _defaultInstance = new KwfRabbitMQExchangeConfiguration
        {
            ExchangeName = "kwf.exchange",
            Durable = true,
            AutoDelete = false
        };

        public string ExchangeName { get; set; } = string.Empty;
        public bool? Durable { get; set; }
        public bool? AutoDelete { get; set; }

        public static KwfRabbitMQExchangeConfiguration GetDefaultInsance() 
        {
            return _defaultInstance;
        }
    }
}
