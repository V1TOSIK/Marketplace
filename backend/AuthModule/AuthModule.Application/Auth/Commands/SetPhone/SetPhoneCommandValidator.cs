using FluentValidation;

namespace AuthModule.Application.Auth.Commands.SetPhone
{
    public class SetPhoneCommandValidator : AbstractValidator<SetPhoneCommand>
    {
        public SetPhoneCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new SetPhoneRequestValidator());
        }
    }
}
