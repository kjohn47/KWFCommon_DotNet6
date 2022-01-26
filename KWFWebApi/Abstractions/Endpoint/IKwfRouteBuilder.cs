namespace KWFWebApi.Abstractions.Endpoint
{

    public interface IKwfRouteBuilder : IKwfRouteStatusBuilder
    {
        /// <summary>
        /// Set Route pattern to be concatenated with global pattern
        /// </summary>
        /// <param name="route">The route pattern</param>
        /// <returns>IKwfRouteStatusBuilder</returns>
        IKwfRouteStatusBuilder SetRoute(string route);
    }
}
