namespace Sample.SampleApi
{
    using KWFAuthentication.Abstractions.Context;

    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Json;

    using KWFWebApi.Abstractions.Logging;
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Extensions;

    using System.Diagnostics;
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    public class WeatherForecastQueryHandler : IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>
    {
        private readonly WeatherForecastServices _service;
        private readonly IKWFLogger<WeatherForecastQueryHandler> _logger;

        public WeatherForecastQueryHandler(
            WeatherForecastServices service, 
            ILoggerFactory loggerFactory, 
            IUserContextAccessor ctx,
            KWFJsonConfiguration jsonCfg)
        {
            _service = service;
            _logger = loggerFactory.CreateKwfLogger<WeatherForecastQueryHandler>();
            _logger.LogInformation(JsonSerializer.Serialize(ctx.GetContext(), jsonCfg.GetJsonSerializerOptions()));
        }

        public Task<ICQRSResult<WeatherForecastQueryResponse>> HandleAsync(WeatherForecastQueryRequest request, CancellationToken? cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var waitMs = Random.Shared.Next(500, 2000);
                var summaries = _service.GetSumaries();

                Task.Delay(waitMs).Wait();

                var forecast = Enumerable.Range(1, 5).Select(index =>
                   new WeatherForecast
                   (
                       DateTime.Now.AddDays(index),
                       Random.Shared.Next(-20, 55),
                       summaries[Random.Shared.Next(summaries.Length)]
                   ));

                return Task.FromResult<ICQRSResult<WeatherForecastQueryResponse>>(
                    CQRSResult<WeatherForecastQueryResponse>
                        .Success(new WeatherForecastQueryResponse
                        {
                            ForecastResults = forecast
                        }));
            }
            finally
            {
                watch.Stop();
                _logger.LogInformation("Service took {0}s to execute", watch.Elapsed.TotalSeconds);
            }
        }
    }
}
