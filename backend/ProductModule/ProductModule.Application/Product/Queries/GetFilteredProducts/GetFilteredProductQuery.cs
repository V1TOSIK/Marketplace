using MediatR;
using ProductModule.Application.Dtos;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQuery : PaginationRequest, IRequest<PaginationResponse<ProductDto>>
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public List<string>? Location { get; set; } = [];
        public string? SortedBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; }
        public int? CategoryId { get; set; }
        public List<CharacteristicFilter>? Characteristics { get; set; } = [];
    }
}
