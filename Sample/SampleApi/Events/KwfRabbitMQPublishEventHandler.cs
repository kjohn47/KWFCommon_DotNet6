namespace Sample.SampleApi.Events
{
    using KWFCaching.Memory.Interfaces;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using Microsoft.Extensions.Logging;
    using Sample.SampleApi.Commands.Events;

    using System.Text.Json;
    using System.Threading.Tasks;

    public class KwfRabbitMQPublishEventHandler : IKwfRabbitMQEventHandler<string>
    {
        private readonly IKwfCacheOnMemory _cache;
        private readonly ILogger? _logger;

        public KwfRabbitMQPublishEventHandler(IKwfCacheOnMemory cache, ILoggerFactory? loggerFactory)
        {
            _cache = cache;
            _logger = loggerFactory?.CreateLogger<KwfRabbitMQPublishEventHandler>();
        }

        public async Task HandleEventAsync(IEventPayloadEnvelope<string> eventData)
        {
            await Task.Run(() =>
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Recieved envelop from rabbitMQ: {ENVELOP}", JsonSerializer.Serialize(eventData));
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
                    Publisher = nameof(EventBusPublisherEnum.RabbitMQ)
                });
                return r.Result;
            });
        }
    }
}
