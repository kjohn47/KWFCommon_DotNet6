namespace Sample.SampleApi.Commands.Events
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFEventBus.KWFKafka.Interfaces;

    using KWFWebApi.Abstractions.Command;

    using Sample.SampleApi.Constants;

    using System.Threading;
    using System.Threading.Tasks;

    public class PublishEventCommandHandler : ICommandHandler<PublishEventCommandRequest, PublishEventCommandResponse>
    {
        private readonly IKwfKafkaBus _eventBus;
        public PublishEventCommandHandler(IKwfKafkaBus eventBus)
        {
            _eventBus = eventBus;
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
            await _eventBus.ProduceAsync(request.EventMessage, AppConstants.TestTopic);
            
            return CQRSResult<PublishEventCommandResponse>.Success(new PublishEventCommandResponse(AppConstants.TestTopic));
        }
    }
}
