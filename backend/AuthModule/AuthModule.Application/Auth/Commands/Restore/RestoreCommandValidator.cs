using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Restore
{
    public class RestoreCommandValidator : AbstractValidator<RestoreCommand>
    {
        public RestoreCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.DeviceId)
                .NotNull().WithMessage("DeviceId cannot be empty.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new RestoreRequestValidator());
        }
    }
}
