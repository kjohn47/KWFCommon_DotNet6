namespace Sample.SampleApi.Events
{
    using System.Text.Json;
    using System.Threading.Tasks;

    using KWFCaching.Memory.Interfaces;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Interfaces;
    using Microsoft.Extensions.Logging;

    using Sample.SampleApi.Commands.Events;

    public class KwfKafkaPublishEventHandler : IKwfKafkaEventHandler<string>
    {
        private readonly IKwfCacheOnMemory _cache;
        private readonly ILogger? _logger;

        public KwfKafkaPublishEventHandler(IKwfCacheOnMemory cache,ILoggerFactory? loggerFactory)
        {
            _cache = cache;
            _logger = loggerFactory?.CreateLogger<KwfKafkaPublishEventHandler>();
        }

        public async Task HandleEventAsync(IEventPayloadEnvelope<string> eventData)
        {
            await Task.Run(() =>
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Recieved envelop from kafka: {ENVELOP}", JsonSerializer.Serialize(eventData));
                }
            });

            await _cache.GetOrInsertCachedItemAsync(
            "EVENT_LIST",
            _ => Task.FromResult(new List<KwfEvent>()),
            r =>
            {
                r.Result.Add(new KwfEvent
                {
                    Id = eventData.Id,
                    TimeStamp = eventData.TimeStamp,
                    Payload = eventData.Payload,
                    Publisher = nameof(EventBusPublisherEnum.Kafka)
                });
                return r.Result;
            });
        }
    }
}
