using MediatR;
using ProductModule.Application.Dtos;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetUserProducts
{
    public class GetUserProductsQuery : PaginationRequest, IRequest<PaginationResponse<ProductDto>>
    {
        public Guid UserId { get; }
        public GetUserProductsQuery(Guid userId, PaginationRequest request) : base(request.PageNumber, request.PageSize)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            UserId = userId;
        }
    }
}
