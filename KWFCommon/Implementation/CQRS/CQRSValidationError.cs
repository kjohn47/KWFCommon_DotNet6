namespace KWFCommon.Implementation.CQRS
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;
    using KWFCommon.Implementation.Models;

    using System.Collections.Generic;
    using System.Net;

    public sealed class CQRSValidationError : ICQRSValidationErrorBuilder
    {
        private readonly IDictionary<string, string> _errors;

        private readonly HttpStatusCode _httpStatusCode;

        private readonly string _errorCode;

        private readonly string _errorMessage;

        private CQRSValidationError(string errorCode, string errorMessage, HttpStatusCode httpStatusCode)
        {
            _errorCode = errorCode;
            _errorMessage = errorMessage;
            _httpStatusCode = httpStatusCode;
            _errors = new Dictionary<string, string>();
        }

        public IErrorResult GetErrorFromValidation()
        {
            return new ErrorResult(_errorCode, _errorMessage, _httpStatusCode, _errors.Select(x => new KeyValuePair<string, string>(x.Key, x.Value)));
        }

        public ICQRSValidationErrorBuilder AddValidationError(string parameter, string message)
        {
            if (!_errors.TryAdd(parameter, message))
            {
                throw new KWFHandledException(_errorCode, _errorMessage, HttpStatusCode.PreconditionFailed, ErrorTypeEnum.Validation);
            }

            return this;
        }

        public ICQRSValidationErrorBuilder AddValidationErrorRange(IDictionary<string, string> errors)
        {
            foreach(var err in errors)
            {
                if (!_errors.TryAdd(err.Key, err.Value))
                {
                    throw new KWFHandledException(_errorCode, _errorMessage, HttpStatusCode.PreconditionFailed, ErrorTypeEnum.Validation);
                }
            }

            return this;
        }

        public static ICQRSValidationErrorBuilder Initialize(string errorCode, string errorMessage)
        {
            return new CQRSValidationError(errorCode, errorMessage, HttpStatusCode.PreconditionFailed);
        }

        public static ICQRSValidationErrorBuilder Initialize(string errorCode, string errorMessage, HttpStatusCode httpStatusCode)
        {
            return new CQRSValidationError(errorCode, errorMessage, httpStatusCode);
        }
    }
}
