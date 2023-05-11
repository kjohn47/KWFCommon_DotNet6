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
            var config = configuration?.GetSection(customConfigurationKey ?? nameof(KwfKafkaConfiguration)).Get<KwfKafkaConfiguration>() ?? null;
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
            services.TryAddSingleton<IKwfKafkaConsumerAccessor, KwfKafkaConsumerAccessor>();
            return services;
        }

        public static IServiceCollection AddKwfKafkaConsumer<THandlerImplementation, TPayload>(this IServiceCollection services, string topic, string? topicConfigurationKey = null)
            where THandlerImplementation : class, IKwfKafkaEventHandler<TPayload>
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

            services.TryAddSingleton<IKwfKafkaEventHandler<TPayload>, THandlerImplementation>();
            services.AddSingleton<IKwfKafkaEventConsumerHandler>(s =>
            {
                return s.GetRequiredService<IKwfKafkaBus>()
                        .CreateConsumer<IKwfKafkaEventHandler<TPayload>, TPayload>(
                            s.GetRequiredService<IKwfKafkaEventHandler<TPayload>>(),
                            topic,
                            topicConfigurationKey);
            });

            return services;
        }

        public static IServiceProvider StartConsumingKafkaEvent<TPayload>(this IServiceProvider services)
            where TPayload : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.GetRequiredService<IKwfKafkaConsumerAccessor>()?.StartConsuming<TPayload>();

            return services;
        }

        public static IServiceProvider StartConsumingAllKafkaEvents(this IServiceProvider services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.GetRequiredService<IKwfKafkaConsumerAccessor>()?.StartConsumingAll();

            return services;
        }
    }
}
