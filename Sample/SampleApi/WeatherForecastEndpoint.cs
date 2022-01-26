namespace Sample.SampleApi
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    public class WeatherForecastEndpoint : EndpointConfigurationBase
    {
        public override void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration)
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
