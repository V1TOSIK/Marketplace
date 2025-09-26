using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreRequestValidator : AbstractValidator<RestoreRequest>
    {
        public RestoreRequestValidator()
        {
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
        }
    }
}
