using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Queries.GetProduct
{
    public class GetProductQuery : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public GetProductQuery(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Product ID cannot be empty.", nameof(productId));
            ProductId = productId;
        }
    }
}
