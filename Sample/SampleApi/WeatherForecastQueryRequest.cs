namespace Sample.SampleApi
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
