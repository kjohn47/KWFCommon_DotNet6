namespace Sample.SampleApi.DependencyInjection
{
    using KWFCaching.Memory.Extensions;
    using KWFCaching.Redis.Extensions;

    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Extensions;
    using KWFEventBus.KWFRabbitMQ.Extensions;
    using KWFEventBus.KWFRabbitMQ.Models;

    using KWFValidation.KWFCQRSValidation.Extensions;
    using KWFValidation.KWFCQRSValidation.Interfaces;

    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Extensions.CQRSHandlers;

    using Sample.SampleApi.Commands.Events;
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
            services.AddKwfKafkaConsumer<KwfKafkaPublishEventHandler, string>(AppConstants.TestTopic);
            /*
            services.AddKwfKafkaConsumer<handler, obj>("kwf.sample.topic.1");
            services.AddKwfKafkaConsumer<handler, obj>("kwf.sample.topic.2");
            */

            //RabbitMQ event bus
            services.AddKwfRabbitMQBus(_configuration);
            services.AddKwfRabbitMQConsumer<KwfRabbitMQPublishEventHandler, string>(AppConstants.TestTopic);
            /*
            services.AddKwfRabbitMQConsumer<handler, obj>("kwf.sample.topic.1");
            services.AddKwfRabbitMQConsumer<handler, obj>("kwf.sample.topic.2");
            */

            // ---- Add common services ----
            services.AddSingleton<IWeatherForecastServices, WeatherForecastServices>();



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



            // ---- Add single validator implementing IKwfCQRSValidator<T> or KwfCQRSValidator<T> with IKwfCQRSValidator<T> as dependency injection type ----
            //services.AddKwfCqrsValidator<PublishEventCommandValidator, PublishEventCommandRequest>(); //default lifetime -> singleton
            //services.AddKwfCqrsValidator<PublishEventCommandValidator, PublishEventCommandRequest>(ServiceLifetime.Transient);

            // ---- Add all validators implementing IKwfCQRSValidator<T> or KwfCQRSValidator<T> on this marker with IKwfCQRSValidator<T> as dependency injection type ----
            services.AddKwfCqrsValidatorsFromAssembly<SampleServiceDefinitions>(); //default lifetime -> singleton
            //services.AddKwfCqrsValidatorsFromAssembly<SampleServiceDefinitions>(ServiceLifetime.Transient);

            // ---- Add all validators implementing IKwfCQRSValidator<T> or KwfCQRSValidator<T> on this markers with IKwfCQRSValidator<T> as dependency injection type ----
            //services.AddKwfCqrsValidatorsFromAssemblies(typeof(SampleServiceDefinitions), typeof(Program)); //lifetime -> singleton
            //services.AddKwfCqrsValidatorsFromAssemblies(ServiceLifetime.Transient, typeof(SampleServiceDefinitions), typeof(Program));
        }

        /*
         * Defaulted on interface, can have implementation ignored
         * 
        */
        public void ConfigureServices(IServiceProvider services)
        {
            //start specific consumer handler for event (handler for type must be registered)
            //services.StartConsumingKafkaEvent<string>();
            //services.StartConsumingRabbitMQEvent<string>();

            //start all registered consumers
            services.StartConsumingAllKafkaEvents();
            services.StartConsumingAllRabbitMQEvents();
        }
    }
}
