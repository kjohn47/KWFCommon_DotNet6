namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFWebApi.Abstractions.Command;
    using KWFWebApi.Abstractions.Query;

    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;

    public interface IKwfEndpointHandler
    {
        /// <summary>
        /// Handle Query for fetching data, will get query handler from Service Collection with type IQueryHandler<TRequest, TResponse>
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <typeparam name="TResponse">Type of query response implementing IQueryResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleQueryAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest 
            where TResponse: IQueryResponse;

        /// <summary>
        /// Handle Query for fetching data
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <typeparam name="TResponse">Type of query response implementing IQueryResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="query">The Query Handler implementing IQueryHandler<TRequest, TResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleQueryAsync<TRequest, TResponse>(
            TRequest request,
            IQueryHandler<TRequest, TResponse> query,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;

        /// <summary>
        /// Handle query that will return file binary, will get query handler from Service Collection with type IQueryHandler<TRequest, IFileQueryResponse>
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleFileQueryAsync<TRequest>(
            TRequest request,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest;

        /// <summary>
        /// Handle query that will return file binary
        /// </summary>
        /// <typeparam name="TRequest">Type of query request implementing IQueryRequest</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="query">The query handler implementing IQueryHandler<TRequest, IFileQueryResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleFileQueryAsync<TRequest>(
            TRequest request,
            IQueryHandler<TRequest, IFileQueryResponse> query,
            CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest;

        /// <summary>
        /// The command Handler to change/create data, will get command handler from Service Collection with type ICommandHandler<TRequest, TResponse>
        /// </summary>
        /// <typeparam name="TRequest">The Command Request type implementing ICommandRequest</typeparam>
        /// <typeparam name="TResponse">The Command Response Type implementing ICommandResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleCommandAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;

        /// <summary>
        /// The command Handler to change/create data 
        /// </summary>
        /// <typeparam name="TRequest">The Command Request type implementing ICommandRequest</typeparam>
        /// <typeparam name="TResponse">The Command Response Type implementing ICommandResponse</typeparam>
        /// <param name="request">The Request</param>
        /// <param name="command">The command Handler implementing ICommandHandler<TRequest, TResponse></param>
        /// <param name="cancellationToken">The Cancellation Token(optional)</param>
        /// <returns>IResult</returns>
        Task<IResult> HandleCommandAsync<TRequest, TResponse>(
            TRequest request,
            ICommandHandler<TRequest, TResponse> command,
            CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;

        /// <summary>
        /// Get Service of type T from Service Collection
        /// </summary>
        /// <typeparam name="T">The service Type</typeparam>
        /// <returns>The Service</returns>
        T GetService<T>() where T : notnull;

        /// <summary>
        /// Get Service provider collection
        /// </summary>
        /// <returns>IServiceProvider</returns>
        IServiceProvider GetServiceProvider();
    }
}
