using FluentValidation;

namespace MediaModule.Application.Media.Commands.DeactivateMedia
{
    public class DeactivateMediaCommandValidator : AbstractValidator<DeactivateMediaCommand>
    {
        public DeactivateMediaCommandValidator()
        {
            RuleFor(x => x.MediaId)
                .NotEmpty().WithMessage("MediaId is required.")
                .NotEqual(Guid.Empty).WithMessage("MediaId cannot be an empty GUID.");
        }
    }
}
