using FluentValidation;

namespace AuthModule.Application.Auth.Commands.SetEmail
{
    public class SetEmailCommandValidator : AbstractValidator<SetEmailCommand>
    {
        public SetEmailCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");


            RuleFor(x => x.Request)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new SetEmailRequestValidator());
        }
    }
}
