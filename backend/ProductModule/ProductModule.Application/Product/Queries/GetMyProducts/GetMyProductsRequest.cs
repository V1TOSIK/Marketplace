using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsRequest : PaginationRequest
    {
        public List<string>? Statuses { get; set; } = [];
    }
}
