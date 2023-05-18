namespace Sample.SampleApi.Events
{

    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using Microsoft.Extensions.Logging;

    using System.Text.Json;
    using System.Threading.Tasks;

    public class KwfRabbitMQPublishEventHandler : IKwfRabbitMQEventHandler<string>
    {
        private readonly ILogger? _logger;
        public KwfRabbitMQPublishEventHandler(ILoggerFactory? loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger<KwfRabbitMQPublishEventHandler>();
        }

        public async Task HandleEventAsync(IEventPayloadEnvelope<string> eventData)
        {
            await Task.Run(() =>
            {
                if (_logger is not null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Recieved envelop: {0}", JsonSerializer.Serialize(eventData));
                }
            });
        }
    }
}
