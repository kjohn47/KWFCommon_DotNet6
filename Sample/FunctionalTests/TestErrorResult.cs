namespace Sample.FunctionalTests
{
    using KWFCommon.Abstractions.Models;

    using System.Collections.Generic;
    using System.Net;

    internal class TestErrorResult : IErrorResult
    {
        public string ErrorCode { get; set; } = string.Empty;

        public string ErrorDescription { get; set; } = string.Empty;

        public IEnumerable<PropertyValidationError>? ValidationErrors { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public ErrorTypeEnum ErrorType { get; set; }
    }
}
