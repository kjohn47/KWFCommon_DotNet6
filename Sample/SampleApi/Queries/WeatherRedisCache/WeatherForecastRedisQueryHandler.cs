namespace Sample.SampleApi.Queries.WeatherRedisCache
{
    using KWFCaching.Redis.Interfaces;

    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFWebApi.Abstractions.Logging;
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Extensions;

    using Sample.SampleApi.Models;
    using Sample.SampleApi.Services;

    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public class WeatherForecastRedisQueryHandler : IQueryHandler<WeatherForecastRedisQueryRequest, WeatherForecastRedisQueryResponse>
    {
        private readonly WeatherForecastServices _service;
        private readonly IKWFLogger<WeatherForecastRedisQueryHandler> _logger;
        private readonly IKwfRedisCache _cache;

        public WeatherForecastRedisQueryHandler(
            WeatherForecastServices service, 
            ILoggerFactory loggerFactory,
            IKwfRedisCache cache)
        {
            _service = service;
            _logger = loggerFactory.CreateKwfLogger<WeatherForecastRedisQueryHandler>();
            _cache = cache;
        }

        public async Task<ICQRSResult<WeatherForecastRedisQueryResponse>> HandleAsync(WeatherForecastRedisQueryRequest request, CancellationToken? cancellationToken)
        {
            if (request.Id is not null && (request.Id < 1 || request.Id > 5))
            {
                return CQRSResult<WeatherForecastRedisQueryResponse>.Failure(
                    new ErrorResult(
                            "INVID",
                            $"Invalid Id {request.Id}, range must be from 1 to 5",
                            HttpStatusCode.BadRequest,
                            ErrorTypeEnum.Validation
                        ));
            }

            var watch = new Stopwatch();
            watch.Start();
            try
            {
                var summaries = await _cache.GetOrInsertCachedItemAsync(
                    "WEATHER_SUMARIES",
                    t => _service.GetSumaries(),
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

                return CQRSResult<WeatherForecastRedisQueryResponse>
                            .Success(new WeatherForecastRedisQueryResponse
                            {
                                ForecastResults = request.Id is not null ? new[] { forecast.ElementAt(request.Id.Value - 1) } : forecast,
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
