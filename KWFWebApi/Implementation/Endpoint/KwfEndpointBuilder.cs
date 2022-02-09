namespace KWFWebApi.Implementation.Endpoint
{
    using KWFAuthentication.Abstractions.Constants;

    using KWFCommon.Abstractions.Constants;
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Implementation.Models;

    using KWFWebApi.Abstractions.Command;
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Query;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Linq;
    using System.Text;
    using System.Text.Json;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class KwfEndpointBuilder : IKwfEndpointBuilder, IKwfEndpointHandler, IKwfEndpointInitialize, IKwfRouteBuilderResult
    {
        private readonly IEndpointRouteBuilder _appBuilder;
        private readonly JsonSerializerOptions _jsonSerializerOpt;
        private string[]? _roles;
        private IHttpContextAccessor? _contextAccessor;

        private KwfEndpointBuilder(IEndpointRouteBuilder appBuilder, JsonSerializerOptions jsonSerializerOpt)
        {
            _appBuilder = appBuilder;
            _jsonSerializerOpt = jsonSerializerOpt;
            BaseUrl = string.Empty;
            _contextAccessor = appBuilder.ServiceProvider.GetService<IHttpContextAccessor>();
        }

        public string BaseUrl { get; private set; }

        /// <summary>
        /// Add MapGet
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder AddGet<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Get);
            routeBuilder(configuration);
            return this;
        }

        /// <summary>
        /// Add MapPost
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder AddPost<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Post);
            routeBuilder(configuration);
            return this;
        }

        /// <summary>
        /// Add MapPut
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder AddPut<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Put);
            routeBuilder(configuration);
            return this;
        }

        /// <summary>
        /// Add MapDelete
        /// </summary>
        /// <typeparam name="TResp">Type of Response</typeparam>
        /// <param name="routeBuilder">The Route builder</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder AddDelete<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Delete);
            routeBuilder(configuration);
            return this;
        }

        public IKwfRouteStatusBuilder AddGet<TResp>(string? route = null)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Get);
            return string.IsNullOrEmpty(route) ? configuration : configuration.SetRoute(route);
        }

        public IKwfRouteStatusBuilder AddPost<TResp>(string? route = null)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Post);
            return string.IsNullOrEmpty(route) ? configuration : configuration.SetRoute(route);
        }

        public IKwfRouteStatusBuilder AddPut<TResp>(string? route = null)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Put);
            return string.IsNullOrEmpty(route) ? configuration : configuration.SetRoute(route);
        }

        public IKwfRouteStatusBuilder AddDelete<TResp>(string? route = null)
        {
            var configuration = new KwfRouteBuilder<TResp>(this, HttpMethodEnum.Delete);
            return string.IsNullOrEmpty(route) ? configuration : configuration.SetRoute(route);
        }

        public IKwfEndpointBuilder AddRouteMethod<TResp>(KwfRouteBuilder<TResp> configuration)
        {
            if (configuration?.Action is null)
            {
                throw new ArgumentNullException(nameof(configuration), "Configuration or action must be set");
            }

            var route = BuildRoute(configuration.Route);
            var map = configuration.HttpMethod switch
            {
                HttpMethodEnum.Get => _appBuilder.MapGet(route, configuration.Action),
                HttpMethodEnum.Post => _appBuilder.MapPost(route, configuration.Action),
                HttpMethodEnum.Put => _appBuilder.MapPut(route, configuration.Action),
                HttpMethodEnum.Delete => _appBuilder.MapDelete(route, configuration.Action),
                _ => throw new NotImplementedException()
            };

            map.WithName(GetOperationId(route, configuration.HttpMethod.GetMethodName()))
               .ConfigureKwfRoute<TResp>(
                                BaseUrl,
                                configuration.SuccessHttpCodes,
                                configuration.ErrorHttpCodes,
                                configuration.Roles ?? (configuration.RemoveGlobalRoles ? null : _roles));

            return this;
        }

        /// <summary>
        /// Handle Query for fetching data, will get query handler from Service Collection with type IQueryHandler<TRequest, TResponse>
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <typeparam name="TResponse">Type of query response implementing IQueryResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public Task<IResult> HandleQueryAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            return HandleQueryAsync(request, GetService<IQueryHandler<TRequest, TResponse>>(), cancellationToken);
        }

        /// <summary>
        /// Handle Query for fetching data
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <typeparam name="TResponse">Type of query response implementing IQueryResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="query">The Query Handler implementing IQueryHandler<TRequest, TResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public async Task<IResult> HandleQueryAsync<TRequest, TResponse>(
            TRequest request,
            IQueryHandler<TRequest, TResponse> query,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            return HandleCQRSResult(await query.HandleAsync(request, cancellationToken));
        }

        /// <summary>
        /// Handle query that will return file binary, will get query handler from Service Collection with type IQueryHandler<TRequest, IFileQueryResponse>
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public Task<IResult> HandleFileQueryAsync<TRequest>(
            TRequest request,
            CancellationToken? cancellationToken = null) where TRequest : IQueryRequest
        {
            return HandleFileQueryAsync(request, GetService<IQueryHandler<TRequest, IFileQueryResponse>>(), cancellationToken);
        }

        /// <summary>
        /// Handle query that will return file binary
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="query">The query handler implementing IQueryHandler<TRequest, IFileQueryResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public async Task<IResult> HandleFileQueryAsync<TRequest>(
            TRequest request,
            IQueryHandler<TRequest, IFileQueryResponse> query,
            CancellationToken? cancellationToken = null) where TRequest : IQueryRequest
        {
            var result = await query.HandleAsync(request, cancellationToken);

            if (result.Error.HasValue)
            {
                return HandleCQRSError(result);
            }

            return Results.File(
                result.Response?.FileBytes?? Array.Empty<byte>(), 
                result.Response?.MimeType, 
                (result.Response?.HasFileName?? false) ? result.Response?.FileName : Guid.NewGuid().ToString());
        }

        /// <summary>
        /// The command Handler to change/create data, will get command handler from Service Collection with type ICommandHandler<TRequest, TResponse>
        /// </summary>
        /// <typeparam name="TRequest">The Command Request type implementing ICommandRequest</typeparam>
        /// <typeparam name="TResponse">The Command Response Type implementing ICommandResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public Task<IResult> HandleCommandAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            return HandleCommandAsync(request, GetService<ICommandHandler<TRequest, TResponse>>(), cancellationToken);
        }

        /// <summary>
        /// The command Handler to change/create data 
        /// </summary>
        /// <typeparam name="TRequest">The Command Request type implementing ICommandRequest</typeparam>
        /// <typeparam name="TResponse">The Command Response Type implementing ICommandResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="command">The command Handler implementing ICommandHandler<TRequest, TResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        public async Task<IResult> HandleCommandAsync<TRequest, TResponse>(
            TRequest request,
            ICommandHandler<TRequest, TResponse> command,
            CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            var validation = await command.ValidateAsync(request, cancellationToken);
            if (validation.HasValue)
            {
                var error = validation.Value.GetErrorFromValidation();
                return Results.Json(error, _jsonSerializerOpt, RestConstants.JsonContentType, (int)error.HttpStatusCode);
            }

            return HandleCQRSResult(await command.ExecuteCommandAsync(request, cancellationToken));
        }

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder InitializeEndpoint(string endpoint)
        {
            BaseUrl = endpoint.EndsWith('/') ? endpoint[0..^1] : endpoint;
            return this;
        }

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="requireAuthorization">The global Require authorized user flag</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, bool requireAuthorization)
        {
            if (requireAuthorization) _roles = Array.Empty<string>();
            return InitializeEndpoint(endpoint);
        }

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="globalAuthorizationPolicy">The global roles authorized for this endpoint</param>
        /// <returns>IKwfEndpointBuilder</returns>
        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, params string[] globalAuthorizationRoles)
        {
            _roles = globalAuthorizationRoles;
            return InitializeEndpoint(endpoint);
        }

        /// <summary>
        /// Initialize endpoint global settings
        /// </summary>
        /// <param name="endpoint">The route pattern</param>
        /// <param name="globalAuthorizationPolicy">The Global Authorization Policy from PoliciesEnum</param>
        /// <returns></returns>
        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, PoliciesEnum globalAuthorizationPolicy)
        {
            return InitializeEndpoint(endpoint, Policies.GetPolicyName(globalAuthorizationPolicy));
        }

        /// <summary>
        /// Get Service of type T from Service Collection
        /// </summary>
        /// <typeparam name="T">The service Type</typeparam>
        /// <returns>The Service</returns>
        public T GetService<T>()
            where T : notnull
        {
            return GetServiceProvider().GetRequiredService<T>();
        }

        public IServiceProvider GetServiceProvider()
        {
            var serviceProvier = _contextAccessor?.HttpContext?.RequestServices;

            if (serviceProvier is null)
            {
                return _appBuilder.ServiceProvider;
            }
            
            return serviceProvier;
        }

        private string BuildRoute(string? route)
        {
            return $"{BaseUrl}/{route}";
        }

        private IResult HandleCQRSResult<TResponse>(ICQRSResult<TResponse> result) 
            where TResponse : ICQRSResponse
        {
            if (result.Error.HasValue)
            {
                return HandleCQRSError(result);
            }

            if (result.HttpStatusCode.HasValue)
            {
                return Results.Json(result.Response, _jsonSerializerOpt, RestConstants.JsonContentType, (int)result.HttpStatusCode.Value);
            }

            return Results.Ok(result.Response);
        }

        private IResult HandleCQRSError<TResponse>(ICQRSResult<TResponse> result)
            where TResponse : ICQRSResponse
        {
            return Results.Json(result.Error.Value, _jsonSerializerOpt, RestConstants.JsonContentType, (int)result.Error.Value.HttpStatusCode);
        }

        private static string GetOperationId(string route, string method)
        {
            var split = route.Split(new[] { '.', '-', '[', ']', '{', '}', '/', '\\', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(method);
            foreach (var s in split)
            {
                stringBuilder.Append(s[0].ToString().ToUpperInvariant());
                if (s.Length > 1)
                {
                    stringBuilder.Append(s[1..]);
                }
            }            

            return stringBuilder.ToString();
        }

        public static IKwfEndpointInitialize CreateEndpointBuilder(IEndpointRouteBuilder builder, JsonSerializerOptions jsonSerializerOpt)
        {
            return new KwfEndpointBuilder(builder, jsonSerializerOpt);
        }
    }

    internal static class KwfResponseHandlerExtensions
    {
        internal static RouteHandlerBuilder ProducesError(this RouteHandlerBuilder builder, int[]? customCodes, bool hasAuthorize)
        {
            if (customCodes is not null && customCodes.Length > 0)
            {
                foreach (var code in customCodes)
                {
                    builder.Produces<ErrorResult>(code);
                }
            }

            builder.Produces<ErrorResult>(400)
                   .Produces<ErrorResult>(404)
                   .Produces<ErrorResult>(500)
                   .Produces<ErrorResult>(501)
                   .Produces<ErrorResult>(503);

            if (hasAuthorize)
            {
                builder.Produces<ErrorResult>(401)
                       .Produces<ErrorResult>(403);
            }

            return builder;
        }

        internal static RouteHandlerBuilder ProducesSuccess<T>(this RouteHandlerBuilder builder, int[]? customCodes)
        {
            if (customCodes is not null && customCodes.Length > 0)
            {
                foreach (var code in customCodes)
                {
                    builder.Produces<T>(code);
                }
            }

            return builder.Produces<T>(200);
        }

        internal static TBuilder RequireKwfAuthorization<TBuilder>(this TBuilder builder, params string[]? roles) where TBuilder : IEndpointConventionBuilder
        {
            if (roles is null)
            {
                return builder;
            }

            if (roles.Length == 0)
            {
                return builder.RequireAuthorization();
            }

            if (roles.Length == 1 && Policies.GetRolesList().Any(x => x.Equals(roles[0])))
            {
                return builder.RequireAuthorization(roles[0]);
            }

            var rolesList = new StringBuilder();
            foreach (var role in roles.Where(x => !x.Equals(Policies.Administrator)))
            {
                rolesList.Append(role);
                rolesList.Append(',');
            }

            rolesList.Append(Policies.Administrator);

            return builder.RequireAuthorization(
                new AuthorizeAttribute()
                {
                    Roles = rolesList.ToString()
                });
        }

        internal static RouteHandlerBuilder ConfigureKwfRoute<T>(this RouteHandlerBuilder builder, string baseUrl, int[]? httpSuccessCodes, int[]? httpErrorCodes, params string[]? roles)
        {
            return builder.WithTags(baseUrl)
                          .ProducesError(httpErrorCodes, roles is not null)
                          .ProducesSuccess<T>(httpSuccessCodes)
                          .RequireKwfAuthorization(roles);
        }
    }
}
