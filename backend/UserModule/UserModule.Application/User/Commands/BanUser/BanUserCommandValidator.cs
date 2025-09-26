using FluentValidation;

namespace UserModule.Application.User.Commands.BanUser
{
    public class BanUserCommandValidator : AbstractValidator<BanUserCommand>
    {
        public BanUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotEqual(Guid.Empty).WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new BanUserRequestValidator());
        }
    }
}
