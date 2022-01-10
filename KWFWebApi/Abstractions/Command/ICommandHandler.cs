namespace KWFWebApi.Abstractions.Command
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;

    public interface ICommandHandler<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        Task<INullableObject<ICQRSValidationError>> ValidateAsync(TRequest request, CancellationToken? cancellationToken);
        Task<ICQRSResult<TResponse>> ExecuteCommandAsync(TRequest request, CancellationToken? cancellationToken);
    }
}
