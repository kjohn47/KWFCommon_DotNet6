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
        builder.AddServiceDefinition(new SampleServiceDefinitions(builder.Configuration)))

    // Add Endpoind definitions classes, they must implement IEndpointConfiguration
    // Method InitializeRoute - return builder.InitializeEndpoint("endpoint name"); -> this is obligatory and defines endpoint base url
    // ConfigureEndpoints - Configures endpoints using minimal api and Kwf extensions that allow using Queries and Command handlers and authentication (based on roles)
    .AddEndpointDefinitionRange(
        new WeatherForecastEndpoint(),
        new ForecastEndpoint())
    .Run();