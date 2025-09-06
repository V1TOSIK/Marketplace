using MediatR;

namespace SharedKernel.Queries
{
    public class GetMainMediasQuery : IRequest<Dictionary<Guid, string>>
    {
        public GetMainMediasQuery(IEnumerable<Guid> entityIds)
        {
            EntityIds = entityIds;
        }
        public IEnumerable<Guid> EntityIds { get; set; } = [];
    }
}
