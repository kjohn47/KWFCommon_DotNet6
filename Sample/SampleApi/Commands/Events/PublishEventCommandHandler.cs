namespace Sample.SampleApi.Commands.Events
{
    using KWFCaching.Memory.Interfaces;

    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFWebApi.Abstractions.Command;

    using Sample.SampleApi.Models;

    using System.Threading;
    using System.Threading.Tasks;

    public class PublishEventCommandHandler : ICommandHandler<PublishEventCommandRequest, PublishEventCommandResponse>
    {
        private readonly IKwfCacheOnMemory _cache;
        public PublishEventCommandHandler(IKwfCacheOnMemory cache)
        {
            _cache = cache;
        }

        public Task<INullableObject<ICQRSValidationError>> ValidateAsync(PublishEventCommandRequest request, CancellationToken? cancellationToken)
        {
            if (string.IsNullOrEmpty(request?.EventMessage))
            {
                return Task.FromResult<INullableObject<ICQRSValidationError>>(
                    NullableObject<ICQRSValidationError>.FromResult(
                        CQRSValidationError.Initialize("INVEVTMSG", "Invalid event message")
                                           .AddValidationError("EventMessage", "Value cannot be null or empty")));
            }

            return Task.FromResult<INullableObject<ICQRSValidationError>>(NullableObject<ICQRSValidationError>.EmptyResult());
        }

        public async Task<ICQRSResult<PublishEventCommandResponse>> ExecuteCommandAsync(PublishEventCommandRequest request, CancellationToken? cancellationToken)
        {
            var newEvent = new KwfEvent(request.EventMessage);
            await _cache.GetOrInsertCachedItemAsync(
                "EVENT_LIST", 
                _ => Task.FromResult(new List<KwfEvent>()),
                r =>
                {
                    r.Result.Add(newEvent);
                    return r.Result;
                });

            return CQRSResult<PublishEventCommandResponse>.Success(new PublishEventCommandResponse(newEvent.Id));
        }
    }
}
