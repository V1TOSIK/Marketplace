using FluentValidation;

namespace AuthModule.Application.OAuth.Commands.OAuthLogin
{
    public class OAuthLoginRequestValidator : AbstractValidator<OAuthLoginRequest>
    {
        public OAuthLoginRequestValidator()
        {
            RuleFor(x => x.ProviderUserId).NotEmpty().WithMessage("ProviderUserId is required.");
            RuleFor(x => x.Provider).NotEmpty().WithMessage("Provider is required.");
        }
    }
}
