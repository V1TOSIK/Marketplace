using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public IEnumerable<Status>? Statuses { get; set; } = [];
    }
}
