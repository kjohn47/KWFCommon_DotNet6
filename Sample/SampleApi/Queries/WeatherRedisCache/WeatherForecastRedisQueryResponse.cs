namespace Sample.SampleApi.Queries.WeatherRedisCache
{
    using KWFWebApi.Abstractions.Query;

    using Sample.SampleApi.Models;

    public class WeatherForecastRedisQueryResponse : IQueryResponse
    {
        public IEnumerable<WeatherForecast>? ForecastResults { get; set; }
    }
}
