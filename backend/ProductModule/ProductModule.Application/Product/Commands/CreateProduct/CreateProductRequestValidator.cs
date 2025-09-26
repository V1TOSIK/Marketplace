using FluentValidation;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Product location is required.")
                .MaximumLength(200).WithMessage("Product location cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Product description cannot exceed 500 characters.");

            RuleFor(x => x.PriceAmount)
                .NotEmpty().WithMessage("Price amount is required.")
                .GreaterThan(0).WithMessage("Price amount must be greater than zero.");

            RuleFor(x => x.PriceCurrency)
                .NotEmpty().WithMessage("Price currency is required.")
                .Length(3).WithMessage("Price currency must be a valid ISO 4217 currency code.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("CategoryId is required.")
                .GreaterThan(0).WithMessage("CategoryId must be a positive integer.");

            RuleForEach(x => x.CharacteristicGroups)
                .SetValidator(new CharacteristicGroupDtoValidator());
        }
    }
}
