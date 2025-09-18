using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public List<string>? Statuses { get; set; } = [];

        public IEnumerable<Status> GetParsedStatuses()
        {
            foreach (var s in Statuses ?? [])
            {
                if (Enum.TryParse<Status>(s, true, out var status))
                    yield return status;
            }
        }
    }
}
