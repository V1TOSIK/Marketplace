using MediatR;

namespace SharedKernel.Queries
{
    public class GetMainMediaQuery : IRequest<string>
    {
        public GetMainMediaQuery(Guid entityId)
        {
            EntityId = entityId;
        }
        public Guid EntityId { get; set; }
    }
}
