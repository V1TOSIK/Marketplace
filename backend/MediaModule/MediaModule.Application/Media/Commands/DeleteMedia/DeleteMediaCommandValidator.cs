using FluentValidation;

namespace MediaModule.Application.Media.Commands.DeleteMedia
{
    public class DeleteMediaCommandValidator : AbstractValidator<DeleteMediaCommand>
    {
        public DeleteMediaCommandValidator()
        {
            RuleFor(x => x.MediaId)
                .NotEmpty().WithMessage("MediaId is required.")
                .NotEqual(Guid.Empty).WithMessage("MediaId cannot be an empty GUID.");
        }
    }
}
