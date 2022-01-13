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
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class KwfEndpointBuilder : IKwfEndpointBuilder, IKwfEndpointHandler, IKwfEndpointInitialize
    {
        private readonly IEndpointRouteBuilder _appBuilder;
        private readonly JsonSerializerOptions _jsonSerializerOpt;
        private string[]? _roles;

        private KwfEndpointBuilder(IEndpointRouteBuilder appBuilder, JsonSerializerOptions jsonSerializerOpt)
        {
            _appBuilder = appBuilder;
            _jsonSerializerOpt = jsonSerializerOpt;
            BaseUrl = string.Empty;
        }

        public string BaseUrl { get; private set; }

        public IKwfEndpointBuilder AddGet<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = BuildRouteConfiguration(routeBuilder);
            if (configuration?.Action is not null)
            {
                _appBuilder.MapGet(BuildRoute(configuration.Route), configuration.Action)
                       .ConfigureKwfRoute<TResp>(
                            configuration.SuccessHttpCodes,
                            configuration.ErrorHttpCodes,
                            configuration.Roles ?? (configuration.RemoveGlobalRoles ? null : _roles));
            }

            return this;
        }

        public IKwfEndpointBuilder AddPost<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = BuildRouteConfiguration(routeBuilder);
            if (configuration?.Action is not null)
            {
                _appBuilder.MapPost(BuildRoute(configuration.Route), configuration.Action)
                       .ConfigureKwfRoute<TResp>(
                            configuration.SuccessHttpCodes,
                            configuration.ErrorHttpCodes,
                            configuration.Roles ?? (configuration.RemoveGlobalRoles ? null : _roles));
            }
            
            return this;
        }

        public IKwfEndpointBuilder AddPut<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = BuildRouteConfiguration(routeBuilder);
            if (configuration?.Action is not null)
            {
                _appBuilder.MapPut(BuildRoute(configuration.Route), configuration.Action)
                       .ConfigureKwfRoute<TResp>(
                            configuration.SuccessHttpCodes,
                            configuration.ErrorHttpCodes,
                            configuration.Roles ?? (configuration.RemoveGlobalRoles ? null : _roles));
            }

            return this;
        }

        public IKwfEndpointBuilder AddDelete<TResp>(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = BuildRouteConfiguration(routeBuilder);
            if (configuration?.Action is not null)
            {
                _appBuilder.MapDelete(BuildRoute(configuration.Route), configuration.Action)
                           .ConfigureKwfRoute<TResp>(
                                configuration.SuccessHttpCodes,
                                configuration.ErrorHttpCodes,
                                configuration.Roles ?? (configuration.RemoveGlobalRoles ? null : _roles));
            }

            return this;
        }

        public Task<IResult> HandleQueryAsync<TRequest, TResponse>(TRequest request, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            return HandleQueryAsync(request, GetService<IQueryHandler<TRequest, TResponse>>(), cancellationToken);
        }

        public async Task<IResult> HandleQueryAsync<TRequest, TResponse>(TRequest request, IQueryHandler<TRequest, TResponse> query, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse
        {
            return HandleCQRSResult(await query.HandleAsync(request, cancellationToken));
        }

        public Task<IResult> HandleFileQueryAsync<TRequest>(TRequest request, CancellationToken? cancellationToken = null) where TRequest : IQueryRequest
        {
            return HandleFileQueryAsync(request, GetService<IQueryHandler<TRequest, IFileQueryResponse>>(), cancellationToken);
        }

        public async Task<IResult> HandleFileQueryAsync<TRequest>(TRequest request, IQueryHandler<TRequest, IFileQueryResponse> query, CancellationToken? cancellationToken = null) where TRequest : IQueryRequest
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

        public Task<IResult> HandleCommandAsync<TRequest, TResponse>(TRequest request, CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse
        {
            return HandleCommandAsync(request, GetService<ICommandHandler<TRequest, TResponse>>(), cancellationToken);
        }

        public async Task<IResult> HandleCommandAsync<TRequest, TResponse>(TRequest request, ICommandHandler<TRequest, TResponse> command, CancellationToken? cancellationToken = null)
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

        public IKwfEndpointBuilder InitializeEndpoint(string endpoint)
        {
            BaseUrl = endpoint.EndsWith('/') ? endpoint[0..^1] : endpoint;
            return this;
        }

        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, bool requireAuthorization)
        {
            if (requireAuthorization) _roles = Array.Empty<string>();
            return InitializeEndpoint(endpoint);
        }

        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, params string[] globalAuthorizationRoles)
        {
            _roles = globalAuthorizationRoles;
            return InitializeEndpoint(endpoint);
        }

        public IKwfEndpointBuilder InitializeEndpoint(string endpoint, PoliciesEnum globalAuthorizationPolicy)
        {
            return InitializeEndpoint(endpoint, Policies.GetPolicyName(globalAuthorizationPolicy));
        }

        private string BuildRoute(string? route)
        {
            return $"{BaseUrl}/{route}";
        }

        private KwfRouteBuilder BuildRouteConfiguration(Func<IKwfRouteBuilder, IKwfRouteBuilderResult> routeBuilder)
        {
            var configuration = new KwfRouteBuilder(this);
            routeBuilder(configuration);

            if (configuration.Action is null)
            {
                throw new ArgumentNullException(nameof(configuration.Action));
            }

            return configuration;
        }

        private T GetService<T>()
            where T : notnull
        {
            return _appBuilder.ServiceProvider.GetRequiredService<T>();
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
                rolesList.AppendLine(",");
            }

            rolesList.Append(Policies.Administrator);

            return builder.RequireAuthorization(
                new AuthorizeAttribute()
                {
                    Roles = rolesList.ToString()
                });
        }

        internal static RouteHandlerBuilder ConfigureKwfRoute<T>(this RouteHandlerBuilder builder, int[]? httpSuccessCodes, int[]? httpErrorCodes, params string[]? roles)
        {
            return builder.ProducesError(httpErrorCodes, roles is not null)
                          .ProducesSuccess<T>(httpSuccessCodes)
                          .RequireKwfAuthorization(roles);
        }
    }
}
