using MediatR;

namespace MediaModule.Application.Media.Commands.DeleteMedia
{
    public class DeleteMediaCommand : IRequest
    {
        public DeleteMediaCommand(Guid mediaId)
        {
            MediaId = mediaId;
        }
        public Guid MediaId { get; set; }
    }
}
