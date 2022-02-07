﻿namespace Sample.SampleApi.Endpoints
{
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using Sample.SampleApi.Queries.WeatherMemoryCache;
    using Sample.SampleApi.Queries.WeatherRedisCache;

    public class WeatherForecastEndpoint : IEndpointConfiguration
    {
        /*
         * Defaulted on interface, can have implementation ignored
         * 

        public IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration) =>
            builder.InitializeEndpoint(nameof(WeatherForecastEndpoint));

        */

        public void ConfigureEndpoints(IKwfEndpointBuilder builder, IConfiguration configuration)
        {
            //Any user
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather")
                    .SetAction(handler => 
                        () => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest())));
            
            //Same as before but using redis cache instead of in memory cache
            builder.AddGet<WeatherForecastRedisQueryResponse>(route =>
                route.SetRoute("get-weather-redis")
                    .SetAction(handler =>
                        () => handler.HandleQueryAsync<WeatherForecastRedisQueryRequest, WeatherForecastRedisQueryResponse>(new WeatherForecastRedisQueryRequest())));

            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather/{id}")
                        .SetAction<int>(handler =>
                        (int id) => handler.HandleQueryAsync<WeatherForecastQueryRequest, WeatherForecastQueryResponse>(new WeatherForecastQueryRequest(id))));

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

            //get query handler from services using handler Get service method (this might be preferable if you need all 16 input parameters for querystring/route)
            //Be sure to always call GetService inside delegate to use http context scoped services
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather-get-service")
                     .SetAction(handler =>
                        () =>
                        {
                            var query = handler.GetService<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>>();
                            return handler.HandleQueryAsync(new WeatherForecastQueryRequest(), query);
                        }));

            //get query as parameter on delegate request method
            builder.AddGet<WeatherForecastQueryResponse>(route =>
                route.SetRoute("get-weather-service-delegate-request")
                     .SetAction<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>>(handler =>
                        (IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse> query) =>
                            handler.HandleQueryAsync(new WeatherForecastQueryRequest(), query)));
        }
    }
}