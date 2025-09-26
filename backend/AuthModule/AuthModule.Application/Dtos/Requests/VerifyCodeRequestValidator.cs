using FluentValidation;

namespace AuthModule.Application.Dtos.Requests
{
    public class VerifyCodeRequestValidator : AbstractValidator<VerifyCodeRequest>
    {
        public VerifyCodeRequestValidator()
        {
            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .MaximumLength(100).WithMessage("Destination must not exceed 100 characters.");
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Code is required.")
                .Length(6).WithMessage("Code must be exactly 6 characters long.");
        }
    }
}
