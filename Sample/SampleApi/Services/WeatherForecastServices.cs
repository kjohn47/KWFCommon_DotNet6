namespace Sample.SampleApi.Services
{
    public class WeatherForecastServices : IWeatherForecastServices
    {
        private readonly string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public Task<string[]> GetSumaries()
        {
            var waitMs = Random.Shared.Next(500, 2000);
            Task.Delay(waitMs).Wait();

            return Task.FromResult(summaries);
        }
    }
}
