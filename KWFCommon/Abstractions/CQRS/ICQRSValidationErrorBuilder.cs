namespace KWFCommon.Abstractions.CQRS
{
    public interface ICQRSValidationErrorBuilder : ICQRSValidationError
    {
        ICQRSValidationErrorBuilder AddValidationError(string parameter, string message);
        ICQRSValidationErrorBuilder AddValidationErrorRange(IDictionary<string, string> errors);
    }
}
