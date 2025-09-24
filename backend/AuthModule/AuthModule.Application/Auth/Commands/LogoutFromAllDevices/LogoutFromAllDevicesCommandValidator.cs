using FluentValidation;

namespace AuthModule.Application.Auth.Commands.LogoutFromAllDevices
{
    public class LogoutFromAllDevicesCommandValidator : AbstractValidator<LogoutFromAllDevicesCommand>
    {
        public LogoutFromAllDevicesCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.")
                .Must(id => id != Guid.Empty).WithMessage("UserId must be a valid GUID.");
        }
    }
}
