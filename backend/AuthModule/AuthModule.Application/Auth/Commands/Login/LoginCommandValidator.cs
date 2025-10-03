using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.DeviceId)
                .NotNull().WithMessage("DeviceId cannot be null.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new LoginRequestValidator());
        }
    }
}
