namespace KWFEventBus.KWFKafka.Models
{
    using Confluent.Kafka;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;

    using System.Collections.Generic;
    using System.Text;

    public class KwfKafkaConfiguration : IKWFEventBusConfiguration
    {
        private string? _endpoints;

        private static IDictionary<string, string> _defaultProducerProps = new Dictionary<string, string>
        {
            {"acks", "all"},
            {"request.timeout.ms", "5000"},
            {"message.timeout.ms", "5000"},
            {"transaction.timeout.ms", "5000"},
            {"socket.timeout.ms", "5000"}
        };

        private static IDictionary<string, string> _defaultConsumerProps = new Dictionary<string, string>
        {
            
            {"enable.auto.commit", "true"},
            {"socket.timeout.ms", "5000"},
            {"session.timeout.ms", "10000"},
            {"auto.commit.interval.ms", "5000" },
            {"allow.auto.create.topics", "true" },
            {"auto.offset.reset", "earliest" }
        };

        public string AppName { get; set; } = string.Empty;

        public string ClientName { get; set; } = Environment.MachineName;

        public int ConsumerTimeout { get; set; } = 5000;

        public int ConsumerMaxRetries { get; set; } = 5;

        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }

        public IEnumerable<EventBusProperty>? CommonProperties { get; set; }

        public IEnumerable<EventBusProperty>? ProducerProperties { get; set; }

        public IEnumerable<EventBusProperty>? ConsumerProperties { get; set; }

        public IDictionary<string, IEnumerable<EventBusProperty>>? TopicConsumerProperties { get; set; }

        public ConsumerConfig GetConsumerConfiguration(string? configurationKey = null)
        {
            if (string.IsNullOrEmpty(AppName))
            {
                throw new KwfKafkaBusException("KWFKAFKAMISSPROP", "Missing kafka AppName, this property is defined as group id for consumer");
            }

            return new ConsumerConfig(GetConsumerProperties(configurationKey))
            {
                BootstrapServers = GetServerEndpoints(),
                GroupId = AppName,
                ClientId = $"{ClientName}.{AppName}"
            };
        }

        public ProducerConfig GetProducerConfiguration()
        {
            return new ProducerConfig(GetProducerProperties())
            {
                BootstrapServers = GetServerEndpoints(),
                ClientId = $"{ClientName}.{AppName}"
            };
        }

        private IDictionary<string, string> GetProducerProperties()
        {
            if (CommonProperties is null && ProducerProperties is null)
            {
                return _defaultProducerProps;
            }

            if (ProducerProperties is null)
            {
                return GetProperties(CommonProperties!, _defaultProducerProps);
            }

            if (CommonProperties is null)
            {
                return GetProperties(ProducerProperties!, _defaultProducerProps);
            }

            return GetProperties(CommonProperties.Union(ProducerProperties), _defaultProducerProps); ;
        }

        private IDictionary<string, string> GetConsumerProperties(string? configurationKey = null)
        {

            if (string.IsNullOrEmpty(configurationKey) && CommonProperties is null && ConsumerProperties is null)
            {
                return _defaultConsumerProps;
            }

            if (!string.IsNullOrEmpty(configurationKey))
            {
                if (TopicConsumerProperties is null || !TopicConsumerProperties.ContainsKey(configurationKey))
                {
                    throw new KwfKafkaBusException("KWFKAFKAMISSPROP", "Missing kafka properties for selected consumer key");
                }

                var topicProps = TopicConsumerProperties[configurationKey];

                if (CommonProperties is null)
                {
                    return GetProperties(topicProps, _defaultConsumerProps);
                }

                return GetProperties(CommonProperties.Union(topicProps), _defaultConsumerProps);
            }

            if (ConsumerProperties is null)
            {
                return GetProperties(CommonProperties!, _defaultConsumerProps);
            }

            if (CommonProperties is null)
            {
                return GetProperties(ConsumerProperties!, _defaultConsumerProps);
            }

            return GetProperties(CommonProperties.Union(ConsumerProperties), _defaultConsumerProps);
        }

        private string GetServerEndpoints()
        {
            if (string.IsNullOrEmpty(_endpoints))
            {
                if (Endpoints is null || !Endpoints.Any())
                {
                    throw new KwfKafkaBusException("MISSKAFKAENDPOINT", "Missing endpoints for kafka bus");
                }

                var strBuilder = new StringBuilder();
                var numEndpoints = Endpoints.Count();
                if (numEndpoints > 1)
                {
                    for (int i = 0; i < numEndpoints - 1; i++)
                    {
                        var endpoint = Endpoints.ElementAt(i);
                        strBuilder.Append(endpoint.Url)
                                  .Append(':')
                                  .Append(endpoint.Port)
                                  .Append(',');
                    }
                }

                var lastEndpoint = Endpoints.ElementAt(numEndpoints - 1);
                strBuilder.Append(lastEndpoint.Url)
                          .Append(':')
                          .Append(lastEndpoint.Port);

                _endpoints = strBuilder.ToString();
            }

            return _endpoints;
        }

        private IDictionary<string, string> GetProperties(IEnumerable<EventBusProperty> properties, IDictionary<string, string> initialProps)
        {
            var dictionary = new Dictionary<string, string>(initialProps);
            foreach (var property in properties)
            {
                if (dictionary.ContainsKey(property.PropertyName))
                {
                    dictionary[property.PropertyName] = property.PropertyValue;
                    continue;
                }

                dictionary.Add(property.PropertyName, property.PropertyValue);
            }

            return dictionary;
        }
    }
}
