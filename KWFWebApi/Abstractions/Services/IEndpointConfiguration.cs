﻿namespace KWFWebApi.Abstractions.Services
{
    using KWFWebApi.Abstractions.Endpoint;

    using Microsoft.Extensions.Configuration;

    public interface IEndpointConfiguration
    {
        /// <summary>
        /// Initialize route, set main route pattern and global authorization
        /// </summary>
        /// <param name="builder">The Initialize endpoint builder</param>
        /// <param name="configuration">The app configuration</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder InitializeRoute(IKwfEndpointInitialize builder, IConfiguration configuration)
        {
            return builder.InitializeEndpoint(this.GetType().Name);
        }

        /// <summary>
        /// Configure endpoints for route pattern
        /// </summary>
        /// <param name="builder">The endpoint builder</param>
        /// <param name="handlers">The endpoint handlers</param>
        /// <param name="configuration">The app configuration</param>
        void ConfigureEndpoints(IKwfEndpointBuilder builder, IKwfEndpointHandler handlers, IConfiguration configuration);
    }
}
