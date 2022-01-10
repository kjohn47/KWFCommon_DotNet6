namespace Sample.SampleApi
{
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Abstractions.Services;

    public class SampleServiceDefinitions : IServiceDefinition
    {
        public SampleServiceDefinitions(IConfiguration configuration, bool isDev)
        {
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<WeatherForecastServices>();
            services.AddTransient<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>, WeatherForecastQueryHandler>();
        }

        public void ConfigureServices(IApplicationBuilder app)
        {
        }
    }
}
