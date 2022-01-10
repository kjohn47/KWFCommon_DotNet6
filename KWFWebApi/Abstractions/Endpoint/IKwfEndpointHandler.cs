namespace KWFWebApi.Abstractions.Endpoint
{
    using KWFWebApi.Abstractions.Command;
    using KWFWebApi.Abstractions.Query;

    using Microsoft.AspNetCore.Http;

    using System.Threading.Tasks;

    public interface IKwfEndpointHandler
    {
        Task<IResult> HandleQueryAsync<TRequest, TResponse>(TRequest request, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest 
            where TResponse: IQueryResponse;
        Task<IResult> HandleQueryAsync<TRequest, TResponse>(TRequest request, IQueryHandler<TRequest, TResponse> query, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest
            where TResponse : IQueryResponse;

        Task<IResult> HandleFileQueryAsync<TRequest>(TRequest request, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest;
        Task<IResult> HandleFileQueryAsync<TRequest>(TRequest request, IQueryHandler<TRequest, IFileQueryResponse> query, CancellationToken? cancellationToken = null)
            where TRequest : IQueryRequest;

        Task<IResult> HandleCommandAsync<TRequest, TResponse>(TRequest request, CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;
        Task<IResult> HandleCommandAsync<TRequest, TResponse>(TRequest request, ICommandHandler<TRequest, TResponse> command, CancellationToken? cancellationToken = null)
            where TRequest : ICommandRequest
            where TResponse : ICommandResponse;
    }
}
