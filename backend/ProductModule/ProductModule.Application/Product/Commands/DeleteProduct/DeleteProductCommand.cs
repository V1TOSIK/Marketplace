using MediatR;

namespace ProductModule.Application.Product.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    {
        public Guid ProductId { get; }
        public DeleteProductCommand(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            ProductId = productId;
        }
    }
}
