namespace Sample.SampleApi.Events
{
    using KWFCaching.Memory.Interfaces;

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Interfaces;
    using System.IdentityModel.Tokens.Jwt;
    using System.Threading.Tasks;

    public class KwfKafkaPublishEventHandler : IKwfKafkaEventHandler<string>
    {
        private readonly IKwfCacheOnMemory _cache;
        public KwfKafkaPublishEventHandler(IKwfCacheOnMemory cache)
        {
            _cache = cache;
        }

        public async Task HandleEventAsync(IEventPayloadEnvelope<string> eventData)
        {
            await _cache.GetOrInsertCachedItemAsync(
            "EVENT_LIST",
            _ => Task.FromResult(new List<KwfEvent>()),
            r =>
            {
                r.Result.Add(new KwfEvent
                {
                    Id = eventData.Id,
                    TimeStamp = eventData.TimeStamp,
                    Payload = eventData.Payload
                });
                return r.Result;
            });
        }
    }
}
