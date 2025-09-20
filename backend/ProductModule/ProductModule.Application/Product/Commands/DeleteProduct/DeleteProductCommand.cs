using MediatR;

namespace ProductModule.Application.Product.Commands.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; }
        public DeleteProductCommand(Guid userId, Guid productId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            UserId = userId;
            ProductId = productId;
        }
    }
}
