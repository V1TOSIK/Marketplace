using FluentValidation;
using ProductModule.Application.Product.Commands.UpdateProduct.CharacteristicUpdates;
using ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates;

namespace ProductModule.Application.Product.Commands.UpdateProduct
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.")
                .When(x => x.Name != null);

            RuleFor(x => x.PriceAmount)
                .GreaterThan(0).WithMessage("Price amount must be greater than zero.")
                .When(x => x.PriceAmount.HasValue);

            RuleFor(x => x.PriceCurrency)
                .MaximumLength(3).WithMessage("Price currency must be a valid 3-letter ISO currency code.")
                .When(x => x.PriceCurrency != null);

            RuleFor(x => x.Location)
                .MaximumLength(200).WithMessage("Location must not exceed 200 characters.")
                .When(x => x.Location != null);

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(x => x.Description != null);

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).When(x => x.CategoryId.HasValue).WithMessage("Category ID must be a positive integer.");

            RuleFor(x => x.Characteristics).SetValidator(new CharacteristicBatchDtoValidator());

            RuleFor(x => x.Groups).SetValidator(new GroupBatchDtoValidator());
        }
    }
}
