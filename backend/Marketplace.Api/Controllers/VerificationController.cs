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

        [HttpPost("send-code")]
        public async Task<IActionResult> SendVerificationCode([FromQuery] string destination, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(destination))
                return BadRequest("Destination cannot be empty.");
            await _verificationService.SendVerificationCode(destination, cancellationToken);
            return Ok("Verification code sent successfully.");
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] VerificationRequest request, CancellationToken cancellationToken)
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
