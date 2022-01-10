namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfEndpointBuilder
    {
        IKwfEndpointBuilder AddGet<TResp>(Action<IKwfRouteBuilder> routeBuilder);
        IKwfEndpointBuilder AddPost<TResp>(Action<IKwfRouteBuilder> routeBuilder);
        IKwfEndpointBuilder AddPut<TResp>(Action<IKwfRouteBuilder> routeBuilder);
        IKwfEndpointBuilder AddDelete<TResp>(Action<IKwfRouteBuilder> routeBuilder);
    }
}
