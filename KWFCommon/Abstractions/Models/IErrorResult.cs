namespace KWFCommon.Abstractions.Models
{
    using System.Net;

    public interface IErrorResult
    {
        string ErrorCode { get; }
        string ErrorDescription { get; }
        IEnumerable<PropertyValidationError>? ValidationErrors { get; }
        HttpStatusCode HttpStatusCode { get; }
        ErrorTypeEnum ErrorType { get; }
    }
}
