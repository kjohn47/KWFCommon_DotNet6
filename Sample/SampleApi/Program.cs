using Sample.SampleApi;
using KWFWebApi.Extensions;

WebApplication.CreateBuilder(args)
    // Build Web application with Kwf authentication, logs and common settings, you can pass custom keys for settings and enable/disable bearer authentication(enabled by default)
    .BuildKwfApplication()

    // you can add more than one provider for same name, name must be set on AppSettings LoggingConfiguration - Providers array
    // Console, Debug, Event and EventSource don't need to be added with AddLoggerProvider, just add them on AppSettings LoggingConfiguration - Providers array
    /*
     * .AddLoggerProvider("Console_2", l => l.AddConsole())
     */

    // Add services implementations classes, they must implement IServiceDefinition - this classes allow to:
    // add services (Dependency Injection)
    // configure services (IApplicationBuilder)
    .AddServiceConfiguration(builder =>
        // ---- Add single instance of IServiceDefinition ----
        //builder.AddServiceDefinition(new SampleServiceDefinitions(builder.Configuration))
        //builder.AddServiceDefinition(new SampleServiceDefinitions2(builder.Configuration)))

        // ---- Add multiple instances of IServiceDefinition ----
        //builder.AddServiceDefinitionRange(
        //    new SampleServiceDefinitions(builder.Configuration)
        //    new SampleServiceDefinitions2(builder.Configuration)
        //    new SampleServiceDefinitions3(builder.Configuration)
        //    ))


        // ---- Add all instances of IServiceDefinition on assemblies of types ----
        //builder.AddServiceDefinitionFromAssemblies(typeof(Program), typeof(OtherTypeOnDifferentAssembly)))

        // ---- Add all instances of IServiceDefinition on assembly of type T ----
        builder.AddServiceDefinitionFromAssembly<Program>())

    // Add Endpoind definitions classes, they must implement IEndpointConfiguration
    // Method InitializeRoute - return builder.InitializeEndpoint("endpoint name"); -> this is obligatory and defines endpoint base url
    // ConfigureEndpoints - Configures endpoints using minimal api and Kwf extensions that allow using Queries and Command handlers and authentication (based on roles)

    // ---- Add Definition from single instance where IEndpointDefinition is implemented ----
    //.AddEndpointDefinition(new WeatherForecastEndpoint())
    //.AddEndpointDefinition(new ForecastEndpoint())

    // ---- Add Endpoint definitions from instance range where IEndpointDefinition is implemented ----
    //.AddEndpointDefinitionRange(
    //    new WeatherForecastEndpoint(),
    //    new ForecastEndpoint())

    // ---- Add from multiple assemblies where class implements IEndpointDefinition ----
    //.AddEndpointDefinitionFromAssemblies(typeof(Program), typeof(OtherTypeOnDifferentAssembly))

    // ---- Add from single assembly from type T where class implements IEndpointDefinition ----
    .AddEndpointDefinitionFromAssembly<Program>()
    .Run();