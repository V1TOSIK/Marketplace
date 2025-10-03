using FluentValidation;

namespace AuthModule.Application.Auth.Commands.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Credential)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Credential is required.")
                    .MaximumLength(100).WithMessage("Credential must not exceed 100 characters.")
                    .Must(IsValidCredential)
                        .WithErrorCode("InvalidCredential")
                        .WithMessage("Credential must be a valid email or phone number.");

            RuleFor(x => x.Password)
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("Password is required.")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
                    .MaximumLength(100).WithMessage("Password must not exceed 100 characters.");
        }
        private bool IsValidCredential(string credential)
        {
            // Simple regex patterns for email and phoneNumber validation
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var phonePattern = @"^\d{9,15}$";
            return System.Text.RegularExpressions.Regex.IsMatch(credential, emailPattern) ||
                   System.Text.RegularExpressions.Regex.IsMatch(credential, phonePattern);
        }
    }
}