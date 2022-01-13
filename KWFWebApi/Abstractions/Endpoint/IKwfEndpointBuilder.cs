namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfEndpointBuilder
    {
        IKwfEndpointBuilder AddGet<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);
        IKwfEndpointBuilder AddPost<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);
        IKwfEndpointBuilder AddPut<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);
        IKwfEndpointBuilder AddDelete<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder);
    }
}
