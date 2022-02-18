namespace Sample.SampleApi.Queries.WeatherMemoryCache
{
    using KWFWebApi.Abstractions.Query;

    using Sample.SampleApi.Models;

    public class WeatherForecastQueryResponse : IQueryResponse
    {
        public IEnumerable<WeatherForecast>? ForecastResults { get; set; }
        public string Money { get; set; } = (100).ToString("C2");
    }
}
