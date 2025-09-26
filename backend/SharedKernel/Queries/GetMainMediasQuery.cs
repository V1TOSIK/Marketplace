using MediatR;
using SharedKernel.Dtos;

namespace SharedKernel.Queries
{
    public class GetMainMediasQuery : IRequest<Dictionary<Guid, MediaDto>>
    {
        public GetMainMediasQuery(List<Guid> entityIds)
        {
            EntityIds = entityIds;
        }
        public List<Guid> EntityIds { get; set; } = [];
    }
}
