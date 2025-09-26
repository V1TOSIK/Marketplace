using FluentValidation;

namespace ProductModule.Application.Dtos
{
    public class CharacteristicGroupDtoValidator : AbstractValidator<CharacteristicGroupDto>
    {
        public CharacteristicGroupDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Characteristic group Id must be a positive integer.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Characteristic group name is required.")
                .MaximumLength(100).WithMessage("Characteristic group name cannot exceed 100 characters.");

            RuleForEach(x => x.Characteristics)
                .SetValidator(new CharacteristicDtoValidator());
        }
    }
}
