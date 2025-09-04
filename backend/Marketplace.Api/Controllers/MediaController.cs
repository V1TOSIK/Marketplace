using MediaModule.Application.Dtos.Requests;
using MediaModule.Application.Dtos.Responses;
using MediaModule.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet("{entityId}")]
        public async Task<ActionResult<IEnumerable<MediaResponse>>> GetAllEntityMedias(Guid entityId, CancellationToken cancellationToken)
        {
            if (entityId == Guid.Empty)
                return BadRequest("Invalid request.");

            var mediaFiles = await _mediaService.GetAllEntityMediaUrls(entityId, cancellationToken);
            if (mediaFiles == null || !mediaFiles.Any())
                return NotFound("No media files found for the specified entity.");

            return Ok(mediaFiles);
        }

        [Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UploadMedia([FromForm] UploadMediaRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.File == null)
                return BadRequest("No files provided for upload.");

            await _mediaService.AddMedia(request, cancellationToken);
            return Ok("Files uploaded successfully.");
        }

        [HttpDelete("{mediaId}")]
        public async Task<ActionResult> DeleteMedia(Guid mediaId, CancellationToken cancellationToken)
        {
            if (mediaId == Guid.Empty)
                return BadRequest("Invalid media ID.");

            await _mediaService.SoftDeleteMedia(mediaId, cancellationToken);
            return NoContent();
        }
    }
}
