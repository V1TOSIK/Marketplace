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
        public async Task<ActionResult<IEnumerable<string>>> GetAllEntityMedias([FromRoute] Guid entityId, CancellationToken cancellationToken)
        {
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
            await _mediator.Send(command, cancellationToken);
            return Ok("Files uploaded successfully.");
        }

        // delete this endpoint and replace to worker service
        [HttpDelete("{mediaId}")]
        public async Task<ActionResult> DeleteMedia([FromRoute] Guid mediaId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeleteMediaCommand(mediaId), cancellationToken);
            return NoContent();
        }
    }
}
