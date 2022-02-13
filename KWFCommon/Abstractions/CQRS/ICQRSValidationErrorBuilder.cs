namespace KWFCommon.Abstractions.CQRS
{
    using KWFCommon.Abstractions.Models;

    public interface ICQRSValidationErrorBuilder : ICQRSValidationError
    {
        ICQRSValidationErrorBuilder AddValidationError(string errorCode, string parameter, string message);
        ICQRSValidationErrorBuilder AddValidationErrorRange(IEnumerable<PropertyValidationError> errors);
    }
}
