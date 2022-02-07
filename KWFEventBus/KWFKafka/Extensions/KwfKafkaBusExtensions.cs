namespace KWFEventBus.KWFKafka.Extensions
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Implementation;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

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

            services.TryAddSingleton<IKwfKafkaBus>(s => new KwfKafkaBus(kwfKafkaConfiguration, EventsJsonOptions.GetJsonOptions()));
            return services;
        }

        public static IServiceCollection AddKwfKafkaConsumer<THandler, TPayload>(this IServiceCollection services, string topic, string? topipConfigurationKey = null)
            where THandler : class, IKwfEventHandler<TPayload>
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

            services.TryAddSingleton<IKwfEventHandler<TPayload>, THandler>();
            services.TryAddSingleton(async s =>
            {
                return await new KwfEventConsumerHandler<THandler, TPayload>(
                    s.GetRequiredService<IKwfEventHandler<TPayload>>(), 
                    s.GetRequiredService<IKwfKafkaBus>())
                .StartConsuming(topic, topipConfigurationKey);
            });

            return services;
        }
    }
}
