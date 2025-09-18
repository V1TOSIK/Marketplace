using MediatR;
using SharedKernel.Dtos;

namespace SharedKernel.Queries
{
    public class GetMainMediasQuery : IRequest<Dictionary<Guid, MediaDto>>
    {
        public GetMainMediasQuery(IEnumerable<Guid> entityIds)
        {
            EntityIds = entityIds;
        }
        public IEnumerable<Guid> EntityIds { get; set; } = [];
    }
}
