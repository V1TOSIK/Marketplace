using FluentValidation;

namespace AuthModule.Application.Auth.Commands.SetPassword
{
    public class SetPasswordRequestValidator : AbstractValidator<SetPasswordRequest>
    {
        public SetPasswordRequestValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("New password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("New password must not exceed 100 characters.");
        }
    }
}
