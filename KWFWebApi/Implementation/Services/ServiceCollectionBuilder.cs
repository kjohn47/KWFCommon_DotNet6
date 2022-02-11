namespace KWFWebApi.Implementation.Services
{
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using System.Reflection;

    internal sealed class ServiceCollectionBuilder : IServiceCollectionBuilder, IAddServiceToCollectionBuilder, IServiceCollectionBuilderReturn
    {
        public ServiceCollectionBuilder(IConfiguration configuration, bool isDev)
        {
            Configuration = configuration;
            IsDev = isDev;
        }

        public IConfiguration Configuration { get; init; }
        public bool IsDev { get; init; }

        public ICollection<IServiceDefinition>? Services { get; private set; }

        public IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition)
        {
            if (Services is null)
            {
                Services = new List<IServiceDefinition>();
            }

            Services.Add(serviceDefinition);
            return this;
        }

        public IServiceCollectionBuilderReturn AddServiceDefinitionRange(params IServiceDefinition[] serviceDefinitions)
        {
            if (serviceDefinitions is null)
            {
                throw new ArgumentNullException(nameof(serviceDefinitions));
            }

            Services = serviceDefinitions;
            
            return this;
        }

        public IServiceCollectionBuilderReturn AddServiceDefinitionFromAssemblies(params Type[] typeInAssembly)
        {
            if (typeInAssembly is null)
            {
                throw new ArgumentNullException(nameof(typeInAssembly));
            }

            var assemblies = typeInAssembly.Select(x => x.Assembly);

            Services = GetServiceDefinitionsFromAssemblies(assemblies.ToArray());

            return this;
        }

        public IServiceCollectionBuilderReturn AddServiceDefinitionFromAssembly<T>()
        {
            var assembly = typeof(T).Assembly;

            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            Services = GetServiceDefinitionsFromAssemblies(assembly);

            return this;
        }

        private ICollection<IServiceDefinition>? GetServiceDefinitionsFromAssemblies(params Assembly[] assemblies)
        {
            var serviceDefinitions = new List<IServiceDefinition> ();

            foreach (var assembly in assemblies)
            {
                var serviceInstallerTypes = assembly.DefinedTypes.Where(a =>
                                            typeof(IServiceDefinition).IsAssignableFrom(a) &&
                                            !a.IsInterface &&
                                            !a.IsAbstract);

                var serviceInstallers = serviceInstallerTypes
                    .Select(x => {
                        var constructorsParams = x.GetConstructors()?
                                                  .FirstOrDefault(x => x.IsPublic && x.GetParameters().Any())?
                                                  .GetParameters();
                        
                        if (constructorsParams is null)
                        {
                            return Activator.CreateInstance(x);
                        }

                        var paramsObj = new List<object>();
                        var isDevAssigned = false;
                        foreach (var cParam in constructorsParams)
                        {
                            var paramType = cParam.ParameterType;
                            if (paramType is null)
                            {
                                continue;
                            }

                            if (paramType.Equals(typeof(IConfiguration)))
                            {
                                paramsObj.Add(Configuration);
                                continue;
                            }
                            
                            if (!isDevAssigned && paramType.Equals(typeof(bool)))
                            {
                                isDevAssigned = true;
                                paramsObj.Add(IsDev);
                                continue;
                            }
                            
                            if (paramType.IsValueType)
                            {
                                paramsObj.Add(Activator.CreateInstance(paramType)!);
                                continue;
                            }

                            paramsObj.Add(null!);
                        }
                        
                        return Activator.CreateInstance(x, paramsObj.ToArray());                        
                    })
                    .Cast<IServiceDefinition>();

                if (serviceInstallers is not null)
                    serviceDefinitions.AddRange(serviceInstallers);
            }

            return serviceDefinitions.Any() ? serviceDefinitions : null;
        }
    }
}
