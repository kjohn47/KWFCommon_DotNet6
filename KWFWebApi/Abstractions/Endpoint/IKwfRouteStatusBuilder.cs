namespace KWFWebApi.Abstractions.Endpoint
{
    public interface IKwfRouteStatusBuilder : IKwfRoutePolicyBuilder
    {
        /// <summary>
        /// Set additional http status codes to be handled as success
        /// </summary>
        /// <param name="codes">The Http Status Codes</param>
        /// <returns>IKwfRouteErrorStatusBuilder</returns>
        IKwfRouteErrorStatusBuilder SetSuccessHttpCodes(params int[] codes);

        /// <summary>
        /// Set additional http status codes to be handled as ErrorResult
        /// </summary>
        /// <param name="codes">The Http Status Codes</param>
        /// <returns>IKwfRouteSuccessStatusBuilder</returns>
        IKwfRouteSuccessStatusBuilder SetErrorHttpCodes(params int[] codes);
    }
}
