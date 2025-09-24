using FluentValidation;

namespace ProductModule.Application.Product.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required.")
                .NotEqual(Guid.Empty).WithMessage("UserId cannot be an empty GUID.");

            RuleFor(x => x.Request)
                .NotNull().WithMessage("Request cannot be null.")
                .SetValidator(new CreateProductRequestValidator());
        }
    }
}
