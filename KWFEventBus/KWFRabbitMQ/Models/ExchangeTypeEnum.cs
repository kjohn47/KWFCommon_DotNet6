namespace KWFEventBus.KWFRabbitMQ.Models
{
    using RabbitMQ.Client;

    public enum ExchangeTypeEnum
    {
        Direct,
        Topic,
        Headers,
        Fanout
    }

    public static class ExchangeTypeEnumExtensions
    {
        public static string GetExchangeType(this ExchangeTypeEnum? exchangeType)
        {
            if (exchangeType == null)
            {
                return ExchangeType.Direct;
            }

            switch (exchangeType)
            {
                case ExchangeTypeEnum.Direct: return ExchangeType.Direct;
                case ExchangeTypeEnum.Topic: return ExchangeType.Topic;
                case ExchangeTypeEnum.Headers: return ExchangeType.Headers;
                case ExchangeTypeEnum.Fanout: return ExchangeType.Fanout;
            }

            return ExchangeType.Direct;
        }

        public static string GetExchangeType(this ExchangeTypeEnum exchangeType)
        {
            return GetExchangeType((ExchangeTypeEnum?) exchangeType);
        }
    }
}
