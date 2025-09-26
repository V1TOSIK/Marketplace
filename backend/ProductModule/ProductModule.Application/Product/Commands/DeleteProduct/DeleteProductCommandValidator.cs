using FluentValidation;

namespace ProductModule.Application.Product.Commands.DeleteProduct
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User ID cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("User ID cannot be an empty GUID.");

            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("Product ID cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("Product ID cannot be an empty GUID.");
        }
    }
}
