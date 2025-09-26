using FluentValidation;

namespace ProductModule.Application.Characteristic.Queries.GetProductCharacterisitcs
{
    public class GetProductCharacteristicsQueryValidator : AbstractValidator<GetProductCharacteristicsQuery>
    {
        public GetProductCharacteristicsQueryValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID cannot be empty.");
        }
    }
}
