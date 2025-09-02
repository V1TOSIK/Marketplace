using MediatR;
using ProductModule.Application.Dtos;

namespace ProductModule.Application.Product.Queries.GetProductByCategory
{
    public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
    {
        public int CategoryId { get; set; }
    }
}
