using FluentValidation;

namespace ProductModule.Application.Product.Commands.PublishProduct
{
    public class PublishProductCommandHandlerValidator : AbstractValidator<PublishProductCommand>
    {
        public PublishProductCommandHandlerValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
        }
    }
}
