using MediatR;
using ProductModule.Application.Dtos;
using ProductModule.Domain.Enums;
using SharedKernel.Pagination;

namespace ProductModule.Application.Product.Queries.GetMyProducts
{
    public class GetMyProductsQuery : PaginationRequest, IRequest<PaginationResponse<ProductDto>>
    {
        public GetMyProductsQuery(Guid userId, List<string>? statuses)
        {
            UserId = userId;
            Statuses = statuses;
        }

        public GetMyProductsQuery(Guid userId, GetMyProductsRequest request) : base(request.PageNumber, request.PageSize)
        {
            UserId = userId;
            Statuses = request.Statuses;
        }

        public Guid UserId { get; set; }
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
