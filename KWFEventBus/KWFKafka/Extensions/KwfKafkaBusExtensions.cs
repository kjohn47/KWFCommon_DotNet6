namespace KWFEventBus.KWFKafka.Extensions
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.KWFKafka.Implementation;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    using System;

    public static class KwfKafkaBusExtensions
    {
        public static IServiceCollection AddKwfKafkaBus(this IServiceCollection services, IConfiguration configuration, string? customConfigurationKey = null)
        {
            var config = configuration?.GetSection(customConfigurationKey ?? "KwfKafkaConfiguration").Get<KwfKafkaConfiguration>() ?? null;
            return services.AddKwfKafkaBus(config!);
        }

        public static IServiceCollection AddKwfKafkaBus(this IServiceCollection services, KwfKafkaConfiguration kwfKafkaConfiguration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (kwfKafkaConfiguration is null)
            {
                throw new ArgumentNullException(nameof(kwfKafkaConfiguration));
            }

            services.TryAddSingleton<IKwfKafkaBus>(s => new KwfKafkaBus(
                kwfKafkaConfiguration, 
                s.GetService<ILoggerFactory>()));
            services.TryAddSingleton<IKwfConsumerAccessor, KwfKafkaConsumerAccessor>();
            return services;
        }

        public static IServiceCollection AddKwfKafkaConsumer<THandlerImplementation, TPayload>(this IServiceCollection services, string topic, string? topicConfigurationKey = null)
            where THandlerImplementation : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentNullException(nameof(topic));
            }

            services.TryAddSingleton<THandlerImplementation>();
            services.AddSingleton<IKwfEventConsumerHandler>(s =>
            {
                return s.GetRequiredService<IKwfKafkaBus>()
                        .CreateConsumer<THandlerImplementation, TPayload>(
                            s.GetRequiredService<THandlerImplementation>(),
                            topic,
                            topicConfigurationKey);
            });

            return services;
        }

        public static IApplicationBuilder StartConsumingKafkaEvent<THandler, TPayload>(this IApplicationBuilder app)
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.ApplicationServices.GetRequiredService<IKwfConsumerAccessor>()?.StartConsuming<THandler, TPayload>();

            return app;
        }

        public static IServiceProvider StartConsumingAllKafkaEvents(this IServiceProvider services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.GetRequiredService<IKwfConsumerAccessor>()?.StartConsumingAll();

            return services;
        }
    }
}
