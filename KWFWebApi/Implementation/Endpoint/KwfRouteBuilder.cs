namespace KWFWebApi.Implementation.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    using KWFWebApi.Abstractions.Endpoint;

    using System;

    internal sealed class KwfRouteBuilder<TResp> : IKwfRouteBuilder, IKwfRouteErrorStatusBuilder, IKwfRouteSuccessStatusBuilder
    {
        private readonly KwfEndpointBuilder _endpointHandler;

        public KwfRouteBuilder(KwfEndpointBuilder endpointHandler, HttpMethodEnum httpMethod)
        {
            _endpointHandler = endpointHandler;
            HttpMethod = httpMethod;
        }

        public HttpMethodEnum HttpMethod { get; init; }
        public string? Route { get; private set; }
        public string[]? Roles { get; private set; }
        public int[]? SuccessHttpCodes { get; private set; }
        public int[]? ErrorHttpCodes { get; private set; }
        public bool RemoveGlobalRoles { get; private set; }

        public IKwfRouteStatusBuilder SetRoute(string route)
        {
            Route = route;
            return this;
        }

        public IKwfRouteSuccessStatusBuilder SetErrorHttpCodes(params int[] codes)
        {
            return AddErrorHttpCodes(codes);
        }

        public IKwfRouteErrorStatusBuilder SetSuccessHttpCodes(params int[] codes)
        {
            return AddSuccessHttpCodes(codes);
        }

        public IKwfRoutePolicyBuilder SetErrorHTTPCodes(params int[] codes)
        {
            return AddErrorHttpCodes(codes);
        }

        public IKwfRoutePolicyBuilder SetSuccessHTTPCodes(params int[] codes)
        {
            return AddSuccessHttpCodes(codes);
        }

        public IKwfRouteActionBuilder SetPolicy(PoliciesEnum role)
        {
            Roles = new[] { Policies.GetPolicyName(role) };
            return this;
        }

        public IKwfRouteActionBuilder SetPolicy(params string[]? roles)
        {
            if (roles is null)
            {
                Roles = Array.Empty<string>();
                return this;
            }

            Roles = roles;
            return this;
        }

        public IKwfRouteActionBuilder DisableGlobalRoles()
        {
            RemoveGlobalRoles = true;
            return this;
        }

        public IKwfRouteBuilderResult SetAction(
            Func<IKwfEndpointHandler, ResultDelegate> action)
        {
            return AddAction(action);
        }
        public IKwfRouteBuilderResult SetAction<T0>(
            Func<IKwfEndpointHandler, ResultDelegate<T0>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> action)
        {
            return AddAction(action);
        }

        public IKwfRouteBuilderResult SetAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Func<IKwfEndpointHandler, ResultDelegate<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> action)
        {
            return AddAction(action);
        }

        private IKwfRouteBuilderResult AddAction(Func<IKwfEndpointHandler, Delegate> action)
        {
            _endpointHandler.AddRouteMethod<TResp>(this, action(_endpointHandler));
            return _endpointHandler;
        }

        private KwfRouteBuilder<TResp> AddSuccessHttpCodes(params int[] codes)
        {
            SuccessHttpCodes = codes;
            return this;
        }

        private KwfRouteBuilder<TResp> AddErrorHttpCodes(params int[] codes)
        {
            ErrorHttpCodes = codes;
            return this;
        }
    }
}
