using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;
        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        [HttpPost("code/send")]
        public async Task<ActionResult> SendVerificationCode([FromQuery] string destination, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(destination))
                return BadRequest("Destination cannot be empty.");
            await _verificationService.SendVerificationCode(destination, cancellationToken);
            return Ok("Verification code sent successfully.");
        }

        [HttpPut("code/verify")]
        public async Task<ActionResult> VerifyCode([FromBody] VerificationRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Destination) || string.IsNullOrWhiteSpace(request.Code))
                return BadRequest("Destination and code cannot be empty.");

            var isVerified = await _verificationService.VerifyCode(request, cancellationToken);
            if (isVerified)
                return Ok("Verification successful.");
            else
                return BadRequest("Invalid verification code or already used.");
        }
    }
}
