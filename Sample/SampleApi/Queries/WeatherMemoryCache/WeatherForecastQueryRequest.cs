namespace Sample.SampleApi.Queries.WeatherMemoryCache
{
    using KWFWebApi.Abstractions.Query;

    public class WeatherForecastQueryRequest : IQueryRequest
    {
        public WeatherForecastQueryRequest()
        { }

        public WeatherForecastQueryRequest(int id)
        {
            Id = id;
        }

        public int? Id { get; set; }
    }
}
