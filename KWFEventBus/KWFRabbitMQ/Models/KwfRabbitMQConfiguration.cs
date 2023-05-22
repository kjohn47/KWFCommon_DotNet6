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
        public int Timeout { get; set; } = 5000;
        public int HeartBeat { get; set; } = 15000;
        public int ConsumerPollingInterval { get; set; } = 5000;
        public bool UsePolling { get; set; } = false;
        public int ProducerAckTimeout { get; set; } = 1500;
        public int ConsumerMaxRetries { get; set; } = -1; // -1 => forever, never stops even on exception / error >1 = stops after retry or reset on success
        public bool RetryConsumerReconnect { get; set; } = true;
        public int ConsumerRetryDelay { get; set; } = 2000;
        public bool AutoQueueCreation { get; set; } = true;
        public bool TopicDurable { get; set; } = true;
        public bool TopicAutoDelete { get; set; } = false;
        public bool TopicExclusive { get; set; } = false;
        public bool TopicWaitAck { get; set; } = true;
        public bool MessagePersistent { get; set; } = true;
        public bool TopicAutoCommit { get; set; } = false;
        public bool TopicRequeueOnFail { get; set; } = false;
        public string DlqTag { get; set; } = "dlq";
        public string DlqExchangeTag { get; set; } = "x"; //dlq exchange is always direct
        public bool EnableDlq { get; set; } = true;
        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }
        public IDictionary<string, KwfRabbitMQTopicConfiguration>? TopicConfiguration { get; set; }

        public string GetClientName()
        {
            return $"{ClientName}.{AppName}";
        }

        public (string ExchangeName, bool Durable, bool AutoDelete, string ExchangeType, IDictionary<string, object>? Arguments) GetExchangeSettings(string? configurationKey)
        {
            string exchangeDefaultName = ExchangeConfiguration?.ExchangeName ?? string.Empty;
            var exchangeDefaultDurable = ExchangeConfiguration?.Durable ?? true;
            var exchangeDefaultAutoDelete = ExchangeConfiguration?.AutoDelete ?? false;
            var exchangeType = ExchangeConfiguration?.Type?.GetExchangeType();
            var args = ExchangeConfiguration?.GetArguments();

            if (string.IsNullOrEmpty(configurationKey))
            {
                return (exchangeDefaultName, exchangeDefaultDurable, exchangeDefaultAutoDelete, exchangeType!, args);
            }

            if (TopicConfiguration is null)
            {
                throw new ArgumentNullException(nameof(TopicConfiguration), _nullTopicSettingsMessage);
            }

            var topicSettings = TopicConfiguration.TryGetValue(configurationKey, out KwfRabbitMQTopicConfiguration? value) ? value : throw new ArgumentNullException(configurationKey, _nonExistentKeyTopicSettingsMessage);

            if (topicSettings.ExchangeConfiguration is null)
            {
                return (exchangeDefaultName, exchangeDefaultDurable, exchangeDefaultAutoDelete, exchangeType!, args);
            }

            return (
                topicSettings.ExchangeConfiguration.ExchangeName,
                topicSettings.ExchangeConfiguration.Durable ?? exchangeDefaultDurable,
                topicSettings.ExchangeConfiguration.AutoDelete ?? exchangeDefaultAutoDelete,
                topicSettings.ExchangeConfiguration.Type.HasValue ? topicSettings.ExchangeConfiguration.Type.GetExchangeType() : exchangeType!,
                topicSettings.ExchangeConfiguration.Arguments is not null ? topicSettings.ExchangeConfiguration.GetArguments() : args);
        }

        public (
            bool MessagePersistent, 
            bool Durable, 
            bool TopicExclusive,
            bool AutoDelete, 
            bool autoTopicCreate,
            bool WaitAck, 
            bool dlqEnabled, 
            bool autoCommit,
            bool requeue,
            IEnumerable<EventBusProperty>? headers,
            IEnumerable<EventBusProperty>? arguments) GetTopicSettings(string? configurationKey)
        {
            var autoCommit = TopicAutoCommit && !EnableDlq && !TopicRequeueOnFail;
            if (string.IsNullOrEmpty(configurationKey))
            {
                return (MessagePersistent, TopicDurable, TopicExclusive, TopicAutoDelete, AutoQueueCreation, TopicWaitAck, EnableDlq, autoCommit, TopicRequeueOnFail, null, null);
            }

            if (TopicConfiguration is null)
            {
                throw new ArgumentNullException(nameof(TopicConfiguration), _nullTopicSettingsMessage);
            }

            var topicSettings = TopicConfiguration.TryGetValue(configurationKey, out KwfRabbitMQTopicConfiguration? value) ? value : throw new ArgumentNullException(configurationKey, _nonExistentKeyTopicSettingsMessage);
            var enableDlq = topicSettings?.EnableDlq ?? EnableDlq;
            var requeue = topicSettings?.RequeueOnFail ?? TopicRequeueOnFail;
            
            if (topicSettings?.AutoCommit is not null)
            {
                if (enableDlq || requeue)
                {
                    autoCommit = false;
                }
                else
                {
                    autoCommit = topicSettings.AutoCommit.Value;
                }
            }

            return (
                topicSettings?.MessagePersistent ?? MessagePersistent,
                topicSettings?.Durable ?? TopicDurable,
                topicSettings?.Exclusive ?? TopicExclusive,
                topicSettings?.AutoDelete ?? TopicAutoDelete,
                topicSettings?.AutoQueueCreation ?? AutoQueueCreation,
                topicSettings?.WaitAck ?? TopicWaitAck,
                enableDlq,
                autoCommit,
                requeue,
                topicSettings?.Headers,
                topicSettings?.Arguments);
        }
    }
}
