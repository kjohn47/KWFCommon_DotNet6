namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteSuccessStatusBuilder : IKwfRoutePolicyBuilder
    {
        /// <summary>
        /// Set additional http status codes to be handled as success
        /// </summary>
        /// <param name="codes">The Http Status Codes</param>
        /// <returns>IKwfRoutePolicyBuilder</returns>
        IKwfRoutePolicyBuilder SetSuccessHTTPCodes(params int[] codes);
    }
}
