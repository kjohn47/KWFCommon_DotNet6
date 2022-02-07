namespace KWFEventBus.KWFKafka.Models
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;

    using System.Collections.Generic;
    using System.Text;

    public class KwfKafkaConfiguration : IKWFEventBusConfiguration
    {
        public string AppName { get; set; } = string.Empty;

        public IEnumerable<EventBusEndpoint>? Endpoints { get; set; }

        public IEnumerable<EventBusProperty>? CommonProperties { get; set; }

        public IEnumerable<EventBusProperty>? ProducerProperties { get; set; }

        public IEnumerable<EventBusProperty>? ConsumerProperties { get; set; }

        public IDictionary<string, IEnumerable<EventBusProperty>>? TopicConsumerProperties { get; set; }

        public IDictionary<string, string>? GetProducerProperties()
        {
            if (CommonProperties is null && ProducerProperties is null)
            {
                return null;
            }

            if (ProducerProperties is null)
            {
                return GetProperties(CommonProperties!);
            }

            if (CommonProperties is null)
            {
                return GetProperties(ProducerProperties!);
            }

            return GetProperties(CommonProperties.Union(ProducerProperties)); ;
        }

        public IDictionary<string, string>? GetConsumerProperties(string? configurationKey = null)
        {

            if (string.IsNullOrEmpty(configurationKey) && CommonProperties is null && ConsumerProperties is null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(configurationKey))
            {
                if (TopicConsumerProperties is null || !TopicConsumerProperties.ContainsKey(configurationKey))
                {
                    //If key is defined, props for key must be set
                    throw new KwfKafkaBusException("KWFKAFKAMISSPROP", "Missing kafka properties for selected consumer key");
                }

                var topicProps = TopicConsumerProperties[configurationKey];

                if (CommonProperties is null)
                {
                    return GetProperties(topicProps);
                }

                return GetProperties(CommonProperties.Union(topicProps));
            }

            if (ConsumerProperties is null)
            {
                return GetProperties(CommonProperties!);
            }

            if (CommonProperties is null)
            {
                return GetProperties(ConsumerProperties!);
            }

            return GetProperties(CommonProperties.Union(ConsumerProperties));
        }

        public string GetServerEndpoints()
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

            return strBuilder.ToString();
        }

        private IDictionary<string, string> GetProperties(IEnumerable<EventBusProperty> properties)
        {
            var dictionary = new Dictionary<string, string>();
            foreach (var property in properties)
            {
                if (dictionary.ContainsKey(property.PropertyName))
                {
                    dictionary[property.PropertyName] = property.PropertyValue;
                }

                dictionary.Add(property.PropertyName, property.PropertyValue);
            }

            return dictionary;
        }
    }
}
