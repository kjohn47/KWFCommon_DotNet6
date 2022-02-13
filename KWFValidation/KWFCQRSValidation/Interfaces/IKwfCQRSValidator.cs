namespace KWFValidation.KWFCQRSValidation.Interfaces
{
    using KWFCommon.Abstractions.CQRS;
    using KWFCommon.Abstractions.Models;

    using System.Threading.Tasks;

    public interface IKwfCQRSValidator<TInput>
    {
        public Task<INullableObject<ICQRSValidationError>> ValidateRequestAsync(TInput request, CancellationToken? cancellationToken = null);
    }
}
