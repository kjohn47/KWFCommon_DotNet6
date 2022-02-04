namespace KWFWebApi.Extensions.CQRSHandlers
{
    using KWFWebApi.Abstractions.Command;
    using KWFWebApi.Abstractions.Query;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class KwfCQRSHandlerExtensions
    {
        public static IServiceCollection AddQueryHandlersFromAssembly<T>(this IServiceCollection services, ServiceLifetime? serviceLifetime = null)
        {
            return services.AddHandlersFromAssemblies(typeof(IQueryHandler<,>), serviceLifetime, typeof(T));
        }

        public static IServiceCollection AddQueryHandlersFromAssemblies(this IServiceCollection services, params Type[] assemblyTypes)
        {
            return services.AddHandlersFromAssemblies(typeof(IQueryHandler<,>), null, assemblyTypes);
        }

        public static IServiceCollection AddQueryHandlersFromAssemblies(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] assemblyTypes)
        {
            return services.AddHandlersFromAssemblies(typeof(IQueryHandler<,>), serviceLifetime, assemblyTypes);
        }

        public static IServiceCollection AddCommandHandlersFromAssembly<T>(this IServiceCollection services, ServiceLifetime? serviceLifetime = null)
        {
            return services.AddHandlersFromAssemblies(typeof(ICommandHandler<,>), serviceLifetime, typeof(T));
        }

        public static IServiceCollection AddCommandHandlersFromAssemblies(this IServiceCollection services, params Type[] assemblyTypes)
        {
            return services.AddHandlersFromAssemblies(typeof(ICommandHandler<,>), null, assemblyTypes);
        }

        public static IServiceCollection AddCommandHandlersFromAssemblies(this IServiceCollection services, ServiceLifetime serviceLifetime, params Type[] assemblyTypes)
        {
            return services.AddHandlersFromAssemblies(typeof(ICommandHandler<,>), serviceLifetime, assemblyTypes);
        }

        private static IServiceCollection AddHandlersFromAssemblies(this IServiceCollection services, Type handlerInterfaceType, ServiceLifetime? serviceLifetime, params Type[] assemblyTypes)
        {
            var lifetime = serviceLifetime ?? ServiceLifetime.Transient;

            foreach (var assembly in assemblyTypes.Select(x => x.Assembly))
            {
                var handlerTypes = assembly.DefinedTypes.Where(a =>
                                            !a.IsInterface &&
                                            !a.IsAbstract &&
                                            a.ImplementedInterfaces.Any(i =>
                                                i.IsGenericType &&
                                                i.GetGenericTypeDefinition().IsAssignableTo(handlerInterfaceType)));

                foreach (var handler in handlerTypes)
                {
                    var handlerInterface = handler.ImplementedInterfaces.First(i => i.GetGenericTypeDefinition().IsAssignableTo(handlerInterfaceType));
                    services.TryAdd(new ServiceDescriptor(handlerInterface, handler, lifetime));
                }
            }

            return services;
        }
    }
}
