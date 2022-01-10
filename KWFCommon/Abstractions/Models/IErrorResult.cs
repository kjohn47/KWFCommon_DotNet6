namespace KWFCommon.Abstractions.Models
{
    using System.Net;

    public interface IErrorResult
    {
        string ErrorCode { get; }
        string ErrorDescription { get; }
        IEnumerable<KeyValuePair<string, string>>? ValidationErrors { get; }
        HttpStatusCode HttpStatusCode { get; }
        ErrorTypeEnum ErrorType { get; }
    }
}
