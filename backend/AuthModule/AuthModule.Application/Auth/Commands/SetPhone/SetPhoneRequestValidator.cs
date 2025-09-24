using FluentValidation;

namespace AuthModule.Application.Auth.Commands.SetPhone
{
    public class SetPhoneRequestValidator : AbstractValidator<SetPhoneRequest>
    {
        public SetPhoneRequestValidator()
        {
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone number is required.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.");
        }
    }
}
