namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteErrorStatusBuilder : IKwfRoutePolicyBuilder
    {
        /// <summary>
        /// Set additional http status codes to be handled as ErrorResult
        /// </summary>
        /// <param name="codes">The Http Status Codes</param>
        /// <returns>IKwfRoutePolicyBuilder</returns>
        IKwfRoutePolicyBuilder SetErrorHTTPCodes(params int[] codes);
    }
}
