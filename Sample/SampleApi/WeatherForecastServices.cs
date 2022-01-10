namespace Sample.SampleApi
{
    public class WeatherForecastServices
    {
        private readonly string[] summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public string[] GetSumaries()
        {
            return summaries;
        }
    }
}
