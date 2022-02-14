namespace KWFValidation.KWFCQRSValidation.Extensions
{
    using KWFCommon.Abstractions.CQRS;

    using KWFValidation.KWFCQRSValidation.Interfaces;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfCQRSValidatorExtensions
    {
        public static IServiceCollection AddKwfCqrsValidator<TValidatorHandler, TInput>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TValidatorHandler : class, IKwfCQRSValidator<TInput>
            where TInput : ICQRSRequest
        {
            services.TryAdd(new ServiceDescriptor(typeof(IKwfCQRSValidator<TInput>), typeof(TValidatorHandler), serviceLifetime));
            return services;
        }

        public static IServiceCollection AddKwfCqrsValidatorsFromAssembly<TMarker>(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
        {
            services.AddValidatorsFromAssemblies(serviceLifetime, typeof(TMarker));
            return services;
        }

        public static IServiceCollection AddKwfCqrsValidatorsFromAssemblies(this IServiceCollection services, params Type[] markers)
        {
            services.AddValidatorsFromAssemblies(ServiceLifetime.Singleton, markers);
            return services;
        }

        public static IServiceCollection AddKwfCqrsValidatorsFromAssemblies(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] markers)
        {
            services.AddValidatorsFromAssemblies(serviceLifetime, markers);
            return services;
        }

        private static void AddValidatorsFromAssemblies(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] types)
        {
            var validatorType = typeof(IKwfCQRSValidator<>);
            foreach (var assembly in types.Select(x => x.Assembly))
            {
                var handlerTypes = assembly.DefinedTypes.Where(a =>
                                            !a.IsInterface &&
                                            !a.IsAbstract &&
                                            a.ImplementedInterfaces.Any(i =>
                                                i.IsGenericType &&
                                                i.GetGenericTypeDefinition().IsAssignableTo(validatorType)));

                foreach (var handler in handlerTypes)
                {
                    var handlerInterface = handler.ImplementedInterfaces.First(i => 
                        i.IsInterface && 
                        i.IsGenericType && 
                        i.GetGenericTypeDefinition().IsAssignableTo(validatorType));

                    services.TryAdd(new ServiceDescriptor(handlerInterface, handler, serviceLifetime));
                }
            }
        }
    }
}
