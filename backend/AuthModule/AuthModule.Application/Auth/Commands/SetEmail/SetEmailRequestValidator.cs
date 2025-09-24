using FluentValidation;

namespace AuthModule.Application.Auth.Commands.SetEmail
{
    public class SetEmailRequestValidator : AbstractValidator<SetEmailRequest>
    {
        public SetEmailRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
