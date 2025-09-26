using FluentValidation;

namespace ProductModule.Application.Product.Commands.UpdateProduct.GroupUpdates
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID is required.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Update request cannot be null.")
                .SetValidator(new UpdateProductRequestValidator());
        }
    }
}
