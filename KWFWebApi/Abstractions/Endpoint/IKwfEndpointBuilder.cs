namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfEndpointBuilder
    {
        /// <summary>
        /// Add MapGet
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder AddGet<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);

        /// <summary>
        /// Add MapPost
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder AddPost<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);

        /// <summary>
        /// Add MapPut
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder AddPut<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);

        /// <summary>
        /// Add MapDelete
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        IKwfEndpointBuilder AddDelete<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);

        IKwfRouteStatusBuilder AddGet<TResp>(string? route = null);
        IKwfRouteStatusBuilder AddPost<TResp>(string? route = null);
        IKwfRouteStatusBuilder AddPut<TResp>(string? route = null);
        IKwfRouteStatusBuilder AddDelete<TResp>(string? route = null);
    }
}
