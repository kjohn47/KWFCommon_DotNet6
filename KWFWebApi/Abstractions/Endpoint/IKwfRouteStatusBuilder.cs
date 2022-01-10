namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteStatusBuilder : IKwfRoutePolicyBuilder
    {
        IKwfRouteErrorStatusBuilder SetSuccessHttpCodes(params int[] codes);
        IKwfRouteSuccessStatusBuilder SetErrorHttpCodes(params int[] codes);
    }
}
