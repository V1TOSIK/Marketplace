using FluentValidation;

namespace AuthModule.Application.Dtos.Requests
{
    public class SendVerificationCodeRequestValidator : AbstractValidator<SendVerificationCodeRequest>
    {
        public SendVerificationCodeRequestValidator()
        {
            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destination is required.")
                .MaximumLength(100).WithMessage("Destination must not exceed 100 characters.");
        }
    }
}
