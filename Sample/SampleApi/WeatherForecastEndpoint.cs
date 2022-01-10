namespace Sample.SampleApi
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    public class WeatherForecastEndpoint : IEndpointConfiguration
    {
        public IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration)
        {
            return builder.InitializeEndpoint("weather-forecast");

            //Initialize with global policy (this is overriden if endpoint has SetPolicy() defined on builder)
            //return builder.InitializeEndpoint("forecast", true);
            //return builder.InitializeEndpoint("forecast", PoliciesEnum.User);
            //return builder.InitializeEndpoint("forecast", "WeatherModerator", "HumanResources", "User");
        }

        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration)
        {
            //Any user
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                      route.SetRoute("get-weather")
                           .SetAction(handler => 
                                () => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest())));
            //Any Authenticated user
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                    route.SetRoute("get-weather-authenticated")
                         .SetPolicy()
                         .SetAction(handler =>
                                () => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest())));

            //User with at least one of the roles
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                    route.SetRoute("get-weather-in-role")
                         .SetPolicy("WeatherModerator", "HumanResources", "User")
                         .SetAction(handler =>
                                () => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest())));
        }
    }
}
