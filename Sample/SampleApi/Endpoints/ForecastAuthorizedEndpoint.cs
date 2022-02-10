namespace Sample.SampleApi.Endpoints
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using Sample.SampleApi.Queries.WeatherMemoryCache;

    public class ForecastAuthorizedEndpoint : IEndpointConfiguration
    {
        //return builder.InitializeEndpoint("forecast");
        //Initialize with global policy (this is overriden if endpoint has SetPolicy() defined on builder)
        public IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration) =>
            builder.InitializeEndpoint(nameof(ForecastAuthorizedEndpoint), true);

        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IKwfEndpointHandler handlers, IConfiguration configuration)
        {
            //Any user - disable global setting
            builder.AddGet<WeatherForecastQueryResponse>("get-weather-no-auth")
                     .DisableGlobalRoles()
                     .SetAction(
                        () => handlers.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest()));

            //Authenticated user in any role as set by global setting
            builder.AddGet<WeatherForecastQueryResponse>("get-weather-authenticated-global")
                     .SetAction(
                        () => handlers.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest()));
        }
    }
}
