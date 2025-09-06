using MediatR;
using Microsoft.AspNetCore.Http;

namespace MediaModule.Application.Media.Commands.UploadMedia
{
    public class UploadMediaCommand : IRequest
    {
        public Guid EntityId { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public bool IsMain { get; set; } = false;
        public IFormFile File { get; set; } = null!;
    }
}
