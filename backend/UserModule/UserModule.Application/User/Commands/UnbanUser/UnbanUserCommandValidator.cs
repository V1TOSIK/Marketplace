using FluentValidation;

namespace UserModule.Application.User.Commands.UnbanUser
{
    public class UnbanUserCommandValidator : AbstractValidator<UnbanUserCommand>
    {
        public UnbanUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");
        }
    }
}
