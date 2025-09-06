using MediatR;

namespace MediaModule.Application.Media.Commands.DeactivateMedia
{
    public class DeactivateMediaCommand : IRequest
    {
        public Guid MediaId { get; set; }
    }
}
