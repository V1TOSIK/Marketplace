using MediatR;

namespace SharedKernel.Queries
{
    public class GetEntityMediasQuery : IRequest<IEnumerable<string>>
    {
        public GetEntityMediasQuery(Guid entityId)
        {
            EntityId = entityId;
        }
        public Guid EntityId { get; set; }
    }
}
