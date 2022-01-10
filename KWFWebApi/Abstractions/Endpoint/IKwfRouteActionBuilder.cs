namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteActionBuilder
    {
        void SetAction(Func<IKwfEndpointHandler, Delegate> action);
    }
}
