using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Queries.GetFilteredProducts
{
    public class GetFilteredProductQuery : IRequest<IEnumerable<ProductDto>>
    {
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public List<string>? Location { get; set; } = [];
        public string? SortedBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; }
        public int? CategoryId { get; set; }
        public List<CharacteristicFilter>? Characteristics { get; set; } = [];
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
