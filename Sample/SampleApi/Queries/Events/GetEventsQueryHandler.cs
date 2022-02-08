namespace Sample.SampleApi.Queries.Events
{
    using KWFCaching.Memory.Interfaces;

    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Implementation.CQRS;

    using KWFWebApi.Abstractions.Query;

    using Sample.SampleApi.Events;

    using System.Threading;
    using System.Threading.Tasks;

    public class GetEventsQueryHandler : IQueryHandler<GetEventsQueryRequest, GetEventsQueryResponse>
    {
        private readonly IKwfCacheOnMemory _cache;
        public GetEventsQueryHandler(IKwfCacheOnMemory cache)
        {
            _cache = cache;
        }

        public Task<ICQRSResult<GetEventsQueryResponse>> HandleAsync(GetEventsQueryRequest request, CancellationToken? cancellationToken)
        {
            var result = _cache.GetCachedItem<List<KwfEvent>>("EVENT_LIST");
            if (result.CacheMiss)
            {
                return Task.FromResult<ICQRSResult<GetEventsQueryResponse>>(
                    CQRSResult<GetEventsQueryResponse>.Success(
                        new GetEventsQueryResponse()));
            }

            if (request.EventId is not null)
            {
                return Task.FromResult<ICQRSResult<GetEventsQueryResponse>>(
                CQRSResult<GetEventsQueryResponse>.Success(
                    new GetEventsQueryResponse(result.Result!.Where(x => x.Id.Equals(request.EventId)))));
            }

            return Task.FromResult<ICQRSResult<GetEventsQueryResponse>>(
                CQRSResult<GetEventsQueryResponse>.Success(new GetEventsQueryResponse(result.Result!)));
        }
    }
}
