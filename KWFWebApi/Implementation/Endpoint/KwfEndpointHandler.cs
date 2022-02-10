namespace KWFWebApi.Implementation.Endpoint
{
    using KWFCommon.Abstractions.Constants;
    using KWFCommon.Abstractions.CQRS;

    using KWFWebApi.Abstractions.Command;
    using KWFWebApi.Abstractions.Endpoint;
    using KWFWebApi.Abstractions.Query;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.DependencyInjection;

    using System.Text.Json;

    internal class KwfEndpointHandler : IKwfEndpointHandler
    {
        private readonly IEndpointRouteBuilder _appBuilder;
        private readonly JsonSerializerOptions _jsonSerializerOpt;
        private IHttpContextAccessor? _contextAccessor;

        public KwfEndpointHandler(IEndpointRouteBuilder appBuilder, JsonSerializerOptions jsonSerializerOpt)
        {
            _appBuilder = appBuilder;
            _jsonSerializerOpt = jsonSerializerOpt;
            _contextAccessor = appBuilder.ServiceProvider.GetService<IHttpContextAccessor>();
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
                result.Response?.FileBytes ?? Array.Empty<byte>(),
                result.Response?.MimeType,
                (result.Response?.HasFileName ?? false) ? result.Response?.FileName : Guid.NewGuid().ToString());
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
    }
}
