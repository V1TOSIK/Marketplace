using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates
{
    public class CharacteristicCreateDtoValidator : AbstractValidator<CharacteristicCreateDto>
    {
        public CharacteristicCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Characteristic name must not be empty.")
                .MaximumLength(100).WithMessage("Characteristic name must not exceed 100 characters.");
            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Characteristic value must not be empty.")
                .MaximumLength(500).WithMessage("Characteristic value must not exceed 500 characters.");
        }
    }
}
