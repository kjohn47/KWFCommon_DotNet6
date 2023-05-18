namespace Sample.SampleApi.Commands.Events
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;
    using KWFEventBus.KWFRabbitMQ.Interfaces;

    using KWFValidation.KWFCQRSValidation.Interfaces;

    using KWFWebApi.Abstractions.Command;

    using Sample.SampleApi.Constants;

    using System.Threading;
    using System.Threading.Tasks;

    public class PublishEventCommandHandler : ICommandHandler<PublishEventCommandRequest, PublishEventCommandResponse>
    {
        //private readonly IKwfKafkaBus _eventBus;
        private readonly IKwfRabbitMQBus _eventBus;
        private readonly IKwfCQRSValidator<PublishEventCommandRequest> _validator;

        public PublishEventCommandHandler(/*IKwfKafkaBus*/IKwfRabbitMQBus eventBus, IKwfCQRSValidator<PublishEventCommandRequest> validator)
        {
            _eventBus = eventBus;
            _validator = validator;
        }

        public Task<INullableObject<ICQRSValidationError>> ValidateAsync(PublishEventCommandRequest request, CancellationToken? cancellationToken)
        {
            return _validator.ValidateRequestAsync(request, cancellationToken);
        }

        public async Task<ICQRSResult<PublishEventCommandResponse>> ExecuteCommandAsync(PublishEventCommandRequest request, CancellationToken? cancellationToken)
        {
            try
            {
                await _eventBus.ProduceAsync(request.EventMessage, AppConstants.TestTopic);
                return CQRSResult<PublishEventCommandResponse>.Success(new PublishEventCommandResponse(AppConstants.TestTopic));
            }
            catch (KwfKafkaBusException ex)
            {
                return CQRSResult<PublishEventCommandResponse>.Failure(new ErrorResult(ex.Code, ex.Message, ErrorTypeEnum.Exception));
            }
            catch
            {
                throw;
            }
        }
    }
}
