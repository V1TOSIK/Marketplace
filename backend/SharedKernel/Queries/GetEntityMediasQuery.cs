using MediatR;
using SharedKernel.Dtos;

namespace SharedKernel.Queries
{
    public class GetEntityMediasQuery : IRequest<IEnumerable<MediaDto>>
    {
        public GetEntityMediasQuery(Guid entityId)
        {
            EntityId = entityId;
        }
        public Guid EntityId { get; set; }
    }
}
