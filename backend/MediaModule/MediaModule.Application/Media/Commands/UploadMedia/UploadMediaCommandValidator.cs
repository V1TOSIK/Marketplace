using FluentValidation;

namespace MediaModule.Application.Media.Commands.UploadMedia
{
    public class UploadMediaCommandValidator : AbstractValidator<UploadMediaCommand>
    {
        public UploadMediaCommandValidator()
        {
            RuleFor(x => x.EntityId)
                .NotEmpty().WithMessage("EntityId is required.")
                .NotEqual(Guid.Empty).WithMessage("EntityId cannot be an empty GUID.");

            RuleFor(x => x.EntityType)
                .NotEmpty().WithMessage("EntityType is required.")
                .MaximumLength(30).WithMessage("EntityType cannot exceed 30 characters.");

            RuleFor(x => x.IsMain)
                .NotNull().WithMessage("IsMain is required.");

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(file => file.Length > 0).WithMessage("File cannot be empty.");
        }
    }
}
