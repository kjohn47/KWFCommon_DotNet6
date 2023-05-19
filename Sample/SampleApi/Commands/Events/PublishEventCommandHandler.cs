namespace Sample.SampleApi.Commands.Events
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using KWFValidation.KWFCQRSValidation.Interfaces;

    using KWFWebApi.Abstractions.Command;

    using Sample.SampleApi.Constants;

    using System.Threading;
    using System.Threading.Tasks;

    public class PublishEventCommandHandler : ICommandHandler<PublishEventCommandRequest, PublishEventCommandResponse>
    {
        private readonly IKwfKafkaBus _kafkaEventBus;
        private readonly IKwfRabbitMQBus _rabbitMQEventBus;
        private readonly IKwfCQRSValidator<PublishEventCommandRequest> _validator;

        public PublishEventCommandHandler(IKwfKafkaBus kafkaEventBus,IKwfRabbitMQBus rabbitMQEventBus, IKwfCQRSValidator<PublishEventCommandRequest> validator)
        {
            _kafkaEventBus = kafkaEventBus;
            _rabbitMQEventBus = rabbitMQEventBus;
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
                switch (request.Publisher)
                {
                    case EventBusPublisherEnum.Kafka:
                        {
                            await _kafkaEventBus.ProduceAsync(request.EventMessage, AppConstants.TestTopic);
                            //await _kafkaEventBus.ProduceMultipleAsync(request.EventMessage, new[] { AppConstants.TestTopic, "kwf.sample.topic.1", "kwf.sample.topic.2" });
                            break;
                        }
                    case EventBusPublisherEnum.RabbitMQ:
                        {
                            await _rabbitMQEventBus.ProduceAsync(request.EventMessage, AppConstants.TestTopic);
                            //await _rabbitMQEventBus.ProduceMultipleAsync(request.EventMessage, new[] { AppConstants.TestTopic, "kwf.sample.topic.1", "kwf.sample.topic.2" });
                            break;
                        }
                }

                return CQRSResult<PublishEventCommandResponse>.Success(new PublishEventCommandResponse(AppConstants.TestTopic));
            }
            catch (KwfKafkaBusException k_ex)
            {
                return CQRSResult<PublishEventCommandResponse>.Failure(new ErrorResult(k_ex.Code, k_ex.Message, ErrorTypeEnum.Exception));
            }
            catch (KwfRabbitMQException r_ex)
            {
                return CQRSResult<PublishEventCommandResponse>.Failure(new ErrorResult(r_ex.Code, r_ex.Message, ErrorTypeEnum.Exception));
            }
            catch
            {
                throw;
            }
        }
    }
}
