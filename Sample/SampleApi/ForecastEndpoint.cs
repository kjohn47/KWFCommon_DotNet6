namespace Sample.SampleApi
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class ForecastEndpoint : IEndpointConfiguration
    {
        public IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration)
        {
            //return builder.InitializeEndpoint("forecast");

            //Initialize with global policy (this is overriden if endpoint has SetPolicy() defined on builder)
            return builder.InitializeEndpoint("forecast", true);
        }

        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration)
        {
            //Any user - disable global setting
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather/{id}")
                     .DisableGlobalRoles()
                     .SetAction<int>(handler =>
                        (int id) => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest(id))));

            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather")
                     .DisableGlobalRoles()
                     .SetAction(handler =>
                        () => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest())));

            //Authenticated user in any role as set by global setting
            //get query handler from services using handler Get service method (this might be preferable if you need all 16 input parameters for querystring/route)
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather-authenticated")
                     .SetAction(handler =>
                     {
                         var query = handler.GetService<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>>();
                         return () => handler.HandleQueryAsync(new WeatherForecastQueryRequest(), query);
                     }));

            //User with at least one of the roles
            //get query as parameter on delegate request method
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather-in-role")
                     .SetPolicy("WeatherModerator", "HumanResources", "User")
                     .SetAction<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>>(handler =>
                        (IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse> query) => 
                            handler.HandleQueryAsync(new WeatherForecastQueryRequest(), query)));
        }
    }
}
