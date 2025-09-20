using MediatR;
using ProductModule.Application.Dtos;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetProductByCategory
{
    public class GetProductsByCategoryQuery : PaginationRequest, IRequest<PaginationResponse<ProductDto>>
    {
        public GetProductsByCategoryQuery(int categoryId, PaginationRequest request) : base(request.PageNumber, request.PageSize)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be a positive integer.", nameof(categoryId));
            CategoryId = categoryId;
        }
        public int CategoryId { get; set; }
    }
}
