using FluentValidation;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserRequestValidator : AbstractValidator<BanUserRequest>
    {
        public BanUserRequestValidator()
        {
            RuleFor(x => x.BanReason)
                .NotEmpty().WithMessage("Ban reason is required.")
                .MaximumLength(500).WithMessage("Ban reason must not exceed 500 characters.");
        }
    }
}
