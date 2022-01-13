namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteActionBuilder
    {
        IKwfRouteBuilderResult SetAction(Func<IKwfEndpointHandler, Delegate> action);
    }
}
