namespace KWFEventBus.KWFKafka.Extensions
{
    using KWFEventBus.Abstractions.Interfaces;
    using KWFEventBus.Abstractions.Models;
    using KWFEventBus.KWFKafka.Implementation;
    using KWFEventBus.KWFKafka.Interfaces;
    using KWFEventBus.KWFKafka.Models;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    using System;
    using System.Text.Json;

    public static class KwfKafkaBusExtensions
    {
        private static readonly JsonSerializerOptions _kafkaJsonSettings = EventsJsonOptions.GetJsonOptions();
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

            services.TryAddSingleton<IKwfKafkaBus>(s => new KwfKafkaBus(kwfKafkaConfiguration, _kafkaJsonSettings));
            services.TryAddSingleton<IKwfKafkaConsumerAcessor, KwfKafkaConsumerAcessor>();
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
            services.AddSingleton<IKwfEventConsumerHandler>(s =>
            {
                return new KwfEventConsumerHandler<THandler, TPayload>(
                    s.GetRequiredService<IKwfEventHandler<TPayload>>(), 
                    s.GetRequiredService<IKwfKafkaBus>(),
                    topic,
                    _kafkaJsonSettings,
                    topipConfigurationKey);
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

            app.ApplicationServices.GetRequiredService<IKwfKafkaConsumerAcessor>()?.StartConsuming<THandler, TPayload>();

            return app;
        }

        public static IApplicationBuilder StartConsumingAllKafkaEvents(this IApplicationBuilder app)
        {
            if (app is null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.ApplicationServices.GetRequiredService<IKwfKafkaConsumerAcessor>()?.StartConsumingAll();          

            return app;
        }
    }
}
