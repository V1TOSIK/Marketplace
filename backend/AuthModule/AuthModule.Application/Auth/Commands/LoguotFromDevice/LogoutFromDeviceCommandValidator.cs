using FluentValidation;

namespace AuthModule.Application.Auth.Commands.LoguotFromDevice
{
    public class LogoutFromDeviceCommandValidator : AbstractValidator<LogoutFromDeviceCommand>
    {
        public LogoutFromDeviceCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token cannot be empty.");
        }
    }
}
