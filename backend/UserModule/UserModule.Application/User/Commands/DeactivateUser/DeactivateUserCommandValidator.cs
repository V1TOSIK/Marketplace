using FluentValidation;

namespace UserModule.Application.User.Commands.DeactivateUser
{
    public class DeactivateUserCommandValidator : AbstractValidator<DeactivateUserCommand>
    {
        public DeactivateUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");
        }
    }
}
