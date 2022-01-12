namespace Sample.SampleApi
{
    using KWFAuthentication.Abstractions.Context;

    using KWFCaching.Memory.Implementation;
    using KWFCaching.Memory.Interfaces;

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
        private readonly IKwfCacheOnMemory _cache;

        public WeatherForecastQueryHandler(
            WeatherForecastServices service, 
            ILoggerFactory loggerFactory, 
            IUserContextAccessor ctx,
            IKwfCacheOnMemory cache,
            KWFJsonConfiguration jsonCfg)
        {
            _service = service;
            _logger = loggerFactory.CreateKwfLogger<WeatherForecastQueryHandler>();
            _cache = cache;

            _logger.LogInformation(JsonSerializer.Serialize(ctx.GetContext(), jsonCfg.GetJsonSerializerOptions()));
        }

        public async Task<ICQRSResult<WeatherForecastQueryResponse>> HandleAsync(WeatherForecastQueryRequest request, CancellationToken? cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var summaries = await _cache.GetOrInsertCachedItemAsync(
                    "WEATHER_SUMARIES",
                    () => _service.GetSumaries(),
                    res => 
                    {
                        _logger.LogInformation(res.CacheMiss ? "Getting data from service" : "Getting data from cache");

                        return res.Result!;
                    });

                var forecast = Enumerable.Range(1, 5).Select(index =>
                   new WeatherForecast
                   (
                       DateTime.Now.AddDays(index),
                       Random.Shared.Next(-20, 55),
                       summaries[Random.Shared.Next(summaries.Length)]
                   ));

                return CQRSResult<WeatherForecastQueryResponse>
                            .Success(new WeatherForecastQueryResponse
                            {
                                ForecastResults = forecast
                            });
            }
            finally
            {
                watch.Stop();
                _logger.LogInformation("Service took {0}s to execute", watch.Elapsed.TotalSeconds);
            }
        }
    }
}
