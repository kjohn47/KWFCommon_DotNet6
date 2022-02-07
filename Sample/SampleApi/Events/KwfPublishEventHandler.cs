namespace Sample.SampleApi.Events
{
    using KWFCaching.Memory.Interfaces;

    using KWFEventBus.Abstractions.Interfaces;

    using System.Threading.Tasks;

    public class KwfPublishEventHandler : IKwfEventHandler<string>
    {
        private readonly IKwfCacheOnMemory _cache;
        public KwfPublishEventHandler(IKwfCacheOnMemory cache)
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
