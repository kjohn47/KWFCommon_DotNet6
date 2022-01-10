namespace KWFWebApi.Abstractions.Query
{
    using KWFCommon.Abstractions.CQRS;

    public interface IQueryHandler<TRequest, TResponse>
        where TRequest : IQueryRequest
        where TResponse : IQueryResponse
    {
        Task<ICQRSResult<TResponse>> HandleAsync(TRequest request, CancellationToken? cancellationToken);
    }
}
