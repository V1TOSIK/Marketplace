using FluentValidation;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.Request).NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new UpdateUserRequestValidator());
        }
    }
}
