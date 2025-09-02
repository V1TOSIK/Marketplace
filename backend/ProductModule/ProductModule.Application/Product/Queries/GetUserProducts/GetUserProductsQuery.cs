using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public Guid UserId { get; }
        public GetUserProductsQuery(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
