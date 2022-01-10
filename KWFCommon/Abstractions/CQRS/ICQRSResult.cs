namespace KWFCommon.Abstractions.CQRS
{
    using KWFCommon.Abstractions.Models;

    using System.Net;

    public interface ICQRSResult<TResponse>
            where TResponse : ICQRSResponse
    {
        INullableObject<IErrorResult> Error { get; }

        HttpStatusCode? HttpStatusCode { get; }

        TResponse? Response { get; }
    }
}
