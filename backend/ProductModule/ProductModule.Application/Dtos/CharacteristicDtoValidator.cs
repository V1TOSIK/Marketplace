using FluentValidation;

namespace ProductModule.Application.Dtos
{
    public class CharacteristicDtoValidator : AbstractValidator<CharacteristicDto>
    {
        public CharacteristicDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Characteristic Id must be a positive integer.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Characteristic name is required.")
                .MaximumLength(100).WithMessage("Characteristic name cannot exceed 100 characters.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Characteristic value is required.")
                .MaximumLength(200).WithMessage("Characteristic value cannot exceed 200 characters.");

            RuleFor(x => x.Unit)
                .MaximumLength(50).WithMessage("Characteristic unit cannot exceed 50 characters.");
        }
    }
}
