using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommandValidator : AbstractValidator<RestoreCommand>
    {
        public RestoreCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new RestoreRequestValidator());
        }
    }
}
