namespace KWFEventBus.KWFRabbitMQ.Extensions
{
    using System;

    using KWFEventBus.KWFRabbitMQ.Implementation;
    using KWFEventBus.KWFRabbitMQ.Interfaces;
    using KWFEventBus.KWFRabbitMQ.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;

    public static class KwfRabbitMQBusExtensions
    {
        public static IServiceCollection AddKwfRabbitMQBus(this IServiceCollection services, IConfiguration configuration, string? customConfigurationKey = null)
        {
            var config = configuration?.GetSection(customConfigurationKey ?? nameof(KwfRabbitMQConfiguration)).Get<KwfRabbitMQConfiguration>() ?? null;
            return services.AddKwfRabbitMQBus(config!);
        }

        public static IServiceCollection AddKwfRabbitMQBus(this IServiceCollection services, KwfRabbitMQConfiguration rabbitMQConfiguration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (rabbitMQConfiguration is null)
            {
                throw new ArgumentNullException(nameof(rabbitMQConfiguration));
            }

            services.TryAddSingleton<IKwfRabbitMQBus>(s => new KwfRabbitMQBus(
                rabbitMQConfiguration,
                s.GetService<ILoggerFactory>()));
            services.TryAddSingleton<IKwfRabbitMQConsumerAccessor, KwfRabbitMQConsumerAccessor>();
            return services;
        }

        public static IServiceCollection AddKwfRabbitMQConsumer<THandlerImplementation, TPayload>(this IServiceCollection services, string topic, string? topicConfigurationKey = null)
            where THandlerImplementation : class, IKwfRabbitMQEventHandler<TPayload>
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

            services.TryAddSingleton<IKwfRabbitMQEventHandler<TPayload>, THandlerImplementation>();
            services.AddSingleton<IKwfRabbitMQConsumerHandler>(s =>
            {
                return s.GetRequiredService<IKwfRabbitMQBus>()
                        .CreateConsumer<IKwfRabbitMQEventHandler<TPayload>, TPayload>(
                            s.GetRequiredService<IKwfRabbitMQEventHandler<TPayload>>(),
                            topic,
                            topicConfigurationKey);
            });

            return services;
        }

        public static IServiceProvider StartConsumingRabbitMQEvent<TPayload>(this IServiceProvider services)
            where TPayload : class
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.GetRequiredService<IKwfRabbitMQConsumerAccessor>()?.StartConsuming<TPayload>();

            return services;
        }

        public static IServiceProvider StartConsumingAllRabbitMQEvents(this IServiceProvider services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.GetRequiredService<IKwfRabbitMQConsumerAccessor>()?.StartConsumingAll();

            return services;
        }
    }
}
