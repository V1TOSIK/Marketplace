using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates
{
    public class CharacteristicUpdateDtoValidator : AbstractValidator<CharacteristicUpdateDto>
    {
        public CharacteristicUpdateDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Characteristic ID must be greater than 0.");

            RuleFor(x => x.Value)
                .MaximumLength(500).WithMessage("Characteristic value must not exceed 500 characters.");

            RuleFor(x => x.Unit)
                .MaximumLength(50).WithMessage("Characteristic unit must not exceed 50 characters.");
        }
    }
}
