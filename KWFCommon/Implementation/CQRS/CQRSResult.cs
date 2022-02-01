namespace KWFCommon.Implementation.CQRS
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.Models;

    using System.Net;

    public sealed class CQRSResult<TResponse> : ICQRSResult<TResponse>
        where TResponse : ICQRSResponse
    {
        private readonly NullableObject<IErrorResult> _error;

        private CQRSResult(
            TResponse response,
            HttpStatusCode? httpStatusCode)
        {
            Response = response;
            HttpStatusCode = httpStatusCode;
            _error = NullableObject<IErrorResult>.EmptyResult();
        }

        private CQRSResult(ErrorResult error)
        {
            HttpStatusCode = error.HttpStatusCode;
            _error = NullableObject<IErrorResult>.FromResult(error);
        }



        public HttpStatusCode? HttpStatusCode { get; }

        public TResponse? Response { get; }

        public INullableObject<IErrorResult> Error => _error;

        public static CQRSResult<TResponse> Success(
            TResponse response,
            HttpStatusCode? httpStatusCode = null)
        {
            return new CQRSResult<TResponse>(response, httpStatusCode);
        }

        public static CQRSResult<TResponse> Failure(ErrorResult error)
        {
            return new CQRSResult<TResponse>(error);
        }
    }
}
