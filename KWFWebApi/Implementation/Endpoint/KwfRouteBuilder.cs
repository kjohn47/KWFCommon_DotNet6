namespace KWFWebApi.Implementation.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    using KWFWebApi.Abstractions.Endpoint;

    using System;

    internal sealed class KwfRouteBuilder : IKwfRouteBuilder, IKwfRouteErrorStatusBuilder, IKwfRouteSuccessStatusBuilder, IKwfRouteBuilderResult
    {
        private readonly IKwfEndpointHandler _handlers;

        public KwfRouteBuilder(IKwfEndpointHandler endpointHandler)
        {
            _handlers = endpointHandler;
        }

        public string? Route { get; private set; }
        public string[]? Roles { get; private set; }
        public Delegate? Action { get; private set; }
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

        public IKwfRouteBuilderResult SetAction(Func<IKwfEndpointHandler, Delegate> action)
        {
            Action = action(_handlers);
            return this;
        }

        private KwfRouteBuilder AddSuccessHttpCodes(params int[] codes)
        {
            SuccessHttpCodes = codes;
            return this;
        }

        public KwfRouteBuilder AddErrorHttpCodes(params int[] codes)
        {
            ErrorHttpCodes = codes;
            return this;
        }
    }
}
