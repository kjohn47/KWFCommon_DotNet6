namespace Sample.SampleApi
{
    using KWFWebApi.Abstractions.Query;

    public class WeatherForecastQueryResponse : IQueryResponse
    {
        public IEnumerable<WeatherForecast>? ForecastResults { get; set; }
    }
}
