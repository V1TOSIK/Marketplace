using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates
{
    public class CharacteristicBatchDtoValidator : AbstractValidator<CharacteristicBatchDto>
    {
        public CharacteristicBatchDtoValidator()
        {
            RuleForEach(x => x.Added)
                .SetValidator(new CharacteristicCreateDtoValidator());

            RuleForEach(x => x.Updated)
                .SetValidator(new CharacteristicUpdateDtoValidator());
        }
    }
}
