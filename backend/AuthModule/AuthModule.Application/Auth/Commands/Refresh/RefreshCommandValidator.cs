using AuthModule.Domain.Entities;
using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Refresh
{
    public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
    {
        public RefreshCommandValidator()
        {
            RuleFor(x => x.DeviceId)
                .NotNull().WithMessage("DeviceId cannot be null.");

            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.");
        }
    }
}
