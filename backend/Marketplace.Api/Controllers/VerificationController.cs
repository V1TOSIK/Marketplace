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
        public async Task<ActionResult> SendVerificationCode([FromQuery] SendVerificationCodeRequest request, CancellationToken cancellationToken)
        {
            await _verificationService.SendVerificationCode(request.Destination, cancellationToken);
            return Ok("Verification code sent successfully.");
        }

        [HttpPut("code/verify")]
        public async Task<ActionResult> VerifyCode([FromBody] VerifyCodeRequest request, CancellationToken cancellationToken)
        {
            var isVerified = await _verificationService.VerifyCode(request, cancellationToken);
            if (isVerified)
                return Ok("Verification successful.");
            else
                return Conflict("Invalid verification code or already used.");
        }
    }
}
