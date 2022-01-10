namespace KWFCommon.Abstractions.CQRS
{
    using KWFCommon.Abstractions.Models;

    public interface ICQRSValidationError
    {
        IErrorResult GetErrorFromValidation();
    }
}
