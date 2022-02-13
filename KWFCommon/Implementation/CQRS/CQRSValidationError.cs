namespace KWFCommon.Implementation.CQRS
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.Models;

    using System.Collections.Generic;
    using System.Net;

    public sealed class CQRSValidationError : ICQRSValidationErrorBuilder
    {
        private readonly List<PropertyValidationError> _errors;

        private readonly HttpStatusCode _httpStatusCode;

        private readonly string _errorCode;

        private readonly string _errorMessage;

        private CQRSValidationError(
            string errorCode,
            string errorMessage,
            HttpStatusCode httpStatusCode)
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _httpStatusCode = httpStatusCode;
            _errors = new List<PropertyValidationError>();
        }

        public IErrorResult GetErrorFromValidation()
        {
            return new ErrorResult(
                _errorCode,
                _errorMessage,
                _httpStatusCode,
                _errors);
        }

        public ICQRSValidationErrorBuilder AddValidationError(string errorCode, string parameter, string message)
        {
            _errors.Add(new PropertyValidationError(parameter, errorCode, message));
            return this;
        }

        public ICQRSValidationErrorBuilder AddValidationErrorRange(IEnumerable<PropertyValidationError> errors)
        {
            _errors.AddRange(errors);
            return this;
        }

        public static ICQRSValidationErrorBuilder Initialize(string errorCode, string errorMessage)
        {
            return new CQRSValidationError(errorCode, errorMessage, HttpStatusCode.PreconditionFailed);
        }

        public static ICQRSValidationErrorBuilder Initialize(
            string errorCode,
            string errorMessage,
            HttpStatusCode httpStatusCode)
        {
            return new CQRSValidationError(errorCode, errorMessage, httpStatusCode);
        }
    }
}
