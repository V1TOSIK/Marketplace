using MediaModule.Application.Media.Commands.DeleteMedia;
using MediaModule.Application.Media.Commands.UploadMedia;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Queries;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        public MediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{entityId}")]
        public async Task<ActionResult<IEnumerable<string>>> GetAllEntityMedias(Guid entityId, CancellationToken cancellationToken)
        {
            if (entityId == Guid.Empty)
                return BadRequest("Invalid request.");

            var mediaUrls = await _mediator.Send(new GetEntityMediasQuery(entityId), cancellationToken);
            if (mediaUrls == null || !mediaUrls.Any())
                return NotFound("No media files found for the specified entity.");

            return Ok(mediaUrls);
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadMedia([FromForm] UploadMediaCommand command, CancellationToken cancellationToken)
        {
            if (command == null || command.File == null)
                return BadRequest("No files provided for upload.");

            await _mediator.Send(command, cancellationToken);
            return Ok("Files uploaded successfully.");
        }

        [HttpDelete("{mediaId}")]
        public async Task<ActionResult> DeleteMedia(Guid mediaId, CancellationToken cancellationToken)
        {
            if (mediaId == Guid.Empty)
                return BadRequest("Invalid media ID.");

            await _mediator.Send(new DeleteMediaCommand(mediaId), cancellationToken);
            return NoContent();
        }
    }
}
