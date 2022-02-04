namespace Sample.SampleApi.Queries.WeatherRedisCache
{
    using KWFWebApi.Abstractions.Query;

    public class WeatherForecastRedisQueryRequest : IQueryRequest
    {
        public WeatherForecastRedisQueryRequest()
        { }

        public WeatherForecastRedisQueryRequest(int id)
        {
            Id = id;
        }

        public int? Id { get; set; }
    }
}
