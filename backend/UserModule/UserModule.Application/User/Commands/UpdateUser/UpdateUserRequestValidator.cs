using FluentValidation;

namespace UserModule.Application.User.Commands.UpdateUser
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

            RuleFor(x => x.PhoneNumbers)
                .Cascade(CascadeMode.Stop)
                .Must(phoneNumbers => phoneNumbers == null || phoneNumbers.All(pn => !string.IsNullOrWhiteSpace(pn)))
                .WithMessage("Phone numbers cannot contain empty or whitespace entries.")
                .ForEach(phoneNumberRule => phoneNumberRule
                    .Matches(@"^\+?[1-9]\d{1,14}$")
                    .WithMessage("Each phone number must be in a valid format."));
        }
    }
}
