namespace KWFCommon.Implementation.Models
{
    using KWFCommon.Abstractions.Models;

    using System;
    using System.Net;

    public sealed class KWFHandledException : Exception
    {
        public KWFHandledException(
            string errorCode,
            string errorMessage,
            HttpStatusCode httpStatusCode,
            ErrorTypeEnum errorType) : base(errorMessage)
        {
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
            ErrorType = errorType;
        }

        public KWFHandledException(
            string errorCode,
            string errorMessage,
            HttpStatusCode httpStatusCode,
            ErrorTypeEnum errorType,
            Exception innerException) : base(errorMessage, innerException)
        {
            ErrorCode = errorCode;
            HttpStatusCode = httpStatusCode;
            ErrorType = errorType;
        }

        public string ErrorCode { get; private set; }
        public HttpStatusCode HttpStatusCode { get; private set; }
        public ErrorTypeEnum ErrorType { get; private set; }
    }
}
