using MediatR;
using SharedKernel.Dtos;

namespace SharedKernel.Queries
{
    public class GetMainMediaQuery : IRequest<MediaDto>
    {
        public GetMainMediaQuery(Guid entityId)
        {
            EntityId = entityId;
        }
        public Guid EntityId { get; set; }
    }
}
