namespace KWFEventBus.KWFRabbitMQ.Models
{
    using System.Collections.Generic;


    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;

    public class KwfRabbitMQConfiguration : IKWFEventBusConfiguration
    {
        private const string _nullTopicSettingsMessage = "TopicConfiguration is null on RabbitMQ settings";
        private const string _nonExistentKeyTopicSettingsMessage = "TopicConfiguration does not contain key on RabbitMQ settings";

        public string AppName { get; set; } = "KWFApp";
        public string ClientName { get; set; } = Environment.MachineName;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public KwfRabbitMQExchangeConfiguration ExchangeConfiguration { get; set; } = KwfRabbitMQExchangeConfiguration.GetDefaultInsance();
        public int Timeout { get; set; } = 10000;
        public int HeartBeat { get; set; } = 20000;
        public int ConsumerPollingInterval { get; set; } = 5000;
        public bool UsePolling { get; set; } = false;
        public int ProducerAckTimeout { get; set; } = 2000;
        public int ConsumerMaxRetries { get; set; } = -1; // -1 => forever, never stops even on exception / error >1 = stops after retry or reset on success
        public bool AutoQueueCreation { get; set; } = true;
        public bool TopicDurable { get; set; } = true;
        public bool TopicAutoDelete { get; set; } = false;
        public bool TopicExclusive { get; set; } = false;
        public bool TopicWaitAck { get; set; } = true;
        public bool MessagePersistent { get; set; } = true;
        public bool TopicAutoCommit { get; set; } = true;
        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }
        public IDictionary<string, KwfRabbitMQTopicConfiguration>? TopicConfiguration { get; set; }

        public string GetClientName()
        {
            return $"{ClientName}.{AppName}";
        }

        public (string ExchangeName, bool Durable, bool AutoDelete) GetExchangeSettings(string? configurationKey)
        {
            string exchangeDefaultName = ExchangeConfiguration?.ExchangeName ?? string.Empty;
            var exchangeDefaultDurable = ExchangeConfiguration?.Durable ?? true;
            var exchangeDefaultAutoDelete = ExchangeConfiguration?.AutoDelete ?? false;

            if (string.IsNullOrEmpty(configurationKey))
            {
                return (exchangeDefaultName, exchangeDefaultDurable, exchangeDefaultAutoDelete);
            }

            if (TopicConfiguration is null)
            {
                throw new ArgumentNullException(nameof(TopicConfiguration), _nullTopicSettingsMessage);
            }

            var topicSettings = TopicConfiguration[configurationKey] ?? throw new ArgumentNullException(configurationKey, _nonExistentKeyTopicSettingsMessage);

            if (topicSettings.ExchangeConfiguration is null)
            {
                return (exchangeDefaultName, exchangeDefaultDurable, exchangeDefaultAutoDelete);
            }

            return (
                topicSettings.ExchangeConfiguration.ExchangeName,
                topicSettings.ExchangeConfiguration.Durable ?? exchangeDefaultDurable,
                topicSettings.ExchangeConfiguration.AutoDelete ?? exchangeDefaultAutoDelete);
        }

        public (bool MessagePersistent, bool Durable, bool TopicExclusive, bool AutoDelete, bool autoTopicCreate, bool WaitAck, bool autoCommit) GetTopicSettings(string? configurationKey)
        {
            if (string.IsNullOrEmpty(configurationKey))
            {
                return (MessagePersistent, TopicDurable, TopicExclusive, TopicAutoDelete, AutoQueueCreation, TopicWaitAck, TopicAutoCommit);
            }

            if (TopicConfiguration is null)
            {
                throw new ArgumentNullException(nameof(TopicConfiguration), _nullTopicSettingsMessage);
            }

            var topicSettings = TopicConfiguration[configurationKey] ?? throw new ArgumentNullException(configurationKey, _nonExistentKeyTopicSettingsMessage);

            return (
                topicSettings.MessagePersistent ?? MessagePersistent,
                topicSettings.Durable ?? TopicDurable,
                topicSettings.Exclusive ?? TopicExclusive,
                topicSettings.AutoDelete ?? TopicAutoDelete,
                topicSettings.AutoQueueCreation ?? AutoQueueCreation,
                topicSettings.WaitAck ?? TopicWaitAck,
                topicSettings.AutoCommit ?? TopicAutoCommit);
        }
    }
}
