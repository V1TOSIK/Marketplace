using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.DeviceId)
                .NotNull().WithMessage("DeviceId cannot be null.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.");

            RuleFor(x => x.Request)
                .SetValidator(new RegisterRequestValidator());
        }
    }
}
