namespace KWFWebApi.Abstractions.Endpoint
{

    public interface IKwfRouteBuilder : IKwfRouteStatusBuilder
    {
        IKwfRouteStatusBuilder SetRoute(string route);
    }
}
