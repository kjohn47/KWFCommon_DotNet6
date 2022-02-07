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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class KwfKafkaBusExtensions
    {
        private static readonly JsonSerializerOptions _eventJsonSettings = EventsJsonOptions.GetJsonOptions();
        public static IServiceCollection AddKwfKafkaBus(this IServiceCollection services, IConfiguration configuration)
        {
            return services;
        }

        public static IServiceCollection AddKwfKafkaBus(this IServiceCollection services, KwfKafkaConfiguration kwfKafkaConfiguration)
        {
            services.TryAddSingleton<IKwfKafkaBus>(s => new KwfKafkaBus(kwfKafkaConfiguration, _eventJsonSettings));
            return services;
        }

        public static IServiceCollection AddKwfKafkaConsumer<THandler, TPayload>(this IServiceCollection services, string topic, string? topipConfigurationKey = null)
            where THandler : class, IKwfEventHandler<TPayload>
            where TPayload : class
        {
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
