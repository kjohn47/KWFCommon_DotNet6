namespace Sample.SampleApi.DependencyInjection
{
    using KWFCaching.Memory.Extensions;
    using KWFCaching.Redis.Extensions;
    using KWFEventBus.KWFKafka.Extensions;

    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Extensions.CQRSHandlers;

    using Sample.SampleApi.Constants;
    using Sample.SampleApi.Events;
    using Sample.SampleApi.Services;

    public class SampleServiceDefinitions : IServiceDefinition
    {
        private readonly IConfiguration _configuration;
        public SampleServiceDefinitions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddServices(IServiceCollection services)
        {
            // ---- Add aditional services ----
            // Memory cache
            services.AddKwfCacheOnMemory(_configuration);
            //Redis distributed cache
            services.AddKwfRedisCache(_configuration);

            //kafka event bus
            services.AddKwfKafkaBus(_configuration);
            //register consumer handler for topic
            services.AddKwfKafkaConsumer<KwfPublishEventHandler, string>(AppConstants.TestTopic);

            // ---- Add common services ----
            services.AddSingleton<WeatherForecastServices>();

            // ---- Add Query and Command handlers one at a time ----
            //services.AddTransient<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>, WeatherForecastQueryHandler>();
            //services.AddTransient<ICommandHandler<WeatherForecastCommandRequest, WeatherForecastCommandResponse>, WeatherForecastCommandHandler>();

            // ---- Add all query and command handlers from selected type assembly as transcient----
            services.AddQueryHandlersFromAssembly<SampleServiceDefinitions>();
            services.AddCommandHandlersFromAssembly<SampleServiceDefinitions>();

            // ---- Add all query and command handlers from multiple type assemblies as transcient----
            //services.AddQueryHandlersFromAssemblies(typeof(SampleServiceDefinitions), typeof(SampleServiceDefinitions_2));
            //services.AddQueryHandlersFromAssemblies(typeof(SampleServiceDefinitions), typeof(SampleServiceDefinitions_2));

            // ---- Add all query and command handlers from selected type assemblies with specific lifetime (default transcient) ----
            //services.AddQueryHandlersFromAssembly<SampleServiceDefinitions>(ServiceLifetime.Scoped);
            //services.AddCommandHandlersFromAssembly<SampleServiceDefinitions>(ServiceLifetime.Scoped);
            //services.AddQueryHandlersFromAssemblies(ServiceLifetime.Scoped, typeof(SampleServiceDefinitions), typeof(SampleServiceDefinitions_2));
            //services.AddQueryHandlersFromAssemblies(ServiceLifetime.Scoped, typeof(SampleServiceDefinitions), typeof(SampleServiceDefinitions_2));
        }

        /*
         * Defaulted on interface, can have implementation ignored
         * 
        */
        public void ConfigureServices(IServiceProvider services)
        {
            //start specific consumer handler for event handler
            //app.StartConsumingKafkaEvent<KwfPublishEventHandler, string>();

            //start all registered consumers
            services.StartConsumingAllKafkaEvents();
        }
    }
}
