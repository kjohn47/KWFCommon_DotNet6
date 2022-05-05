namespace KWFCommon.Implementation.Models
{
    using KWFCommon.Abstractions.Models;

    using System.Collections.Generic;
    using System.Net;

    public sealed class ErrorResult : IErrorResult
    {
        public ErrorResult() : this(string.Empty, string.Empty, HttpStatusCode.InternalServerError, ErrorTypeEnum.Unknown, null)
        { }

        public ErrorResult(string errorCode) 
            : this(errorCode, string.Empty, HttpStatusCode.InternalServerError, ErrorTypeEnum.Unknown, null)
        {
        }

        public ErrorResult(string errorCode, string errorDescription) 
            : this(errorCode, errorDescription, HttpStatusCode.InternalServerError, ErrorTypeEnum.Unknown, null)
        {
        }

        public ErrorResult(string errorCode, HttpStatusCode httpStatus) 
            : this(errorCode, string.Empty, httpStatus, ErrorTypeEnum.Unknown, null)
        {
        }

        public ErrorResult(string errorCode, ErrorTypeEnum type) 
            : this(errorCode, string.Empty, HttpStatusCode.InternalServerError, type, null)
        {
        }

        public ErrorResult(string errorCode, string errorDescription, HttpStatusCode httpStatus) 
            : this(errorCode, errorDescription, httpStatus, ErrorTypeEnum.Unknown, null)
        {
        }

        public ErrorResult(string errorCode, string errorDescription, ErrorTypeEnum type) 
            : this(errorCode, errorDescription, HttpStatusCode.InternalServerError, type, null)
        {
        }

        public ErrorResult(string errorCode, string errorDescription, HttpStatusCode httpStatus, IEnumerable<PropertyValidationError> validationErrors) 
            : this(errorCode, errorDescription, httpStatus, ErrorTypeEnum.Validation, validationErrors)
        {
        }

        public ErrorResult(string errorCode, string errorDescription, HttpStatusCode httpStatus, ErrorTypeEnum type) 
            : this(errorCode, errorDescription, httpStatus, type, null)
        {
        }

        public ErrorResult(
            string errorCode,
            string errorDescription,
            HttpStatusCode httpStatus,
            ErrorTypeEnum type,
            IEnumerable<PropertyValidationError>? validationErrors)
        {
            ErrorCode = errorCode;
            ErrorDescription = errorDescription;
            HttpStatusCode = httpStatus;
            ErrorType = type;
            ValidationErrors = validationErrors;
        }

        public string ErrorCode { get; init; }
        public string ErrorDescription { get; init; }
        public HttpStatusCode HttpStatusCode { get; init; }
        public ErrorTypeEnum ErrorType { get; init; }
        public IEnumerable<PropertyValidationError>? ValidationErrors { get; init; }
    }
}
