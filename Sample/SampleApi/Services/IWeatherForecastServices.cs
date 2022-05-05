namespace Sample.SampleApi.Services
{
    public interface IWeatherForecastServices
    {
        Task<string[]> GetSumaries();
    }
}
