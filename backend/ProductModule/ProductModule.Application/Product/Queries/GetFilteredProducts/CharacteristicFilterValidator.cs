using FluentValidation;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class CharacteristicFilterValidator : AbstractValidator<CharacteristicFilter>
    {
        public CharacteristicFilterValidator()
        {
            RuleFor(x => x.TemplateId)
                .GreaterThan(0).WithMessage("TemplateId must be greater than 0.");

            RuleFor(x => x.Values)
                .ForEach(valueRule => valueRule
                    .NotEmpty().WithMessage("Each value must not be empty.")
                    .MaximumLength(100).WithMessage("Each value must not exceed 100 characters."));
        }
    }
}