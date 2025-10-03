using FluentValidation;

namespace AuthModule.Application.OAuth.Commands.OAuthLogin
{
    public class OAuthLoginCommandValidator : AbstractValidator<OAuthLoginCommand>
    {
        public OAuthLoginCommandValidator()
        {
            RuleFor(x => x.ProviderUserId)
                .NotEmpty().WithMessage("ProviderUserId is required.");

            RuleFor(x => x.Provider)
                .NotEmpty().WithMessage("Provider is required.");

            RuleFor(x => x.Email)
                .EmailAddress()
                .When(x => !string.IsNullOrWhiteSpace(x.Email)).WithMessage("Email is not valid.");

            RuleFor(x => x.DeviceId)
                .NotNull().WithMessage("DeviceId cannot be null.");
        }
    }
}
