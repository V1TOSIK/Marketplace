using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public IEnumerable<string>? Statuses { get; set; } = Array.Empty<string>();

        public IEnumerable<Status> GetParsedStatuses()
        {
            foreach (var s in Statuses ?? Array.Empty<string>())
            {
                if (Enum.TryParse<Status>(s, true, out var status))
                    yield return status;
            }
        }
    }
}
