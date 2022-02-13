namespace Sample.SampleApi.Commands.Events
{
    using KWFValidation.KWFCQRSValidation.Implementation;
    using FluentValidation;

    public class PublishEventCommandValidator : KwfCQRSValidator<PublishEventCommandRequest>
    {
        public PublishEventCommandValidator()
            : base("INVEVTMSG", "Invalid event message")
        {
            RuleFor(x => x)
                .NotNull()
                .WithErrorCode("MISSINGREQ")
                .WithMessage("Missing request");

            When(x => (x is not null), () => {
                RuleFor(x => x.EventMessage)
                    .NotEmpty()
                    .WithErrorCode("MISSINGEVTMSG")
                    .WithMessage("Value for message cannot be null or empty");
            });
        }
    }
}
