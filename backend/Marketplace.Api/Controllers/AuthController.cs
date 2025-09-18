using AuthModule.Application.Auth.Commands.AddEmail;
using AuthModule.Application.Auth.Commands.AddPhone;
using AuthModule.Application.Auth.Commands.ChangePassword;
using AuthModule.Application.Auth.Commands.Login;
using AuthModule.Application.Auth.Commands.LogoutFromAllDevices;
using AuthModule.Application.Auth.Commands.LoguotFromDevice;
using AuthModule.Application.Auth.Commands.Refresh;
using AuthModule.Application.Auth.Commands.Register;
using AuthModule.Application.Auth.Commands.Restore;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ICookieService _cookieService;
        private readonly IMediator _mediator;

        public AuthController(
            ICookieService cookieService,
            IMediator mediator)
        {
            _cookieService = cookieService;
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthorizeResponse>> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                return BadRequest("Password cannot be empty or null");

            if (string.IsNullOrWhiteSpace(command.Credential))
                return BadRequest("Cretential must be provided");

            var result = await _mediator.Send(command, cancellationToken);

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthorizeResponse>> Login([FromBody] LoginCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Password))
                return BadRequest("Password cannot be empty or null");

            if (string.IsNullOrWhiteSpace(command.Credential))
                return BadRequest("Either email or phone number must be provided");

            var result = await _mediator.Send(command, cancellationToken);

            if (result.RefreshToken != null)
            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }

        [HttpPut("{userId}/restore")]
        public async Task<ActionResult<AuthorizeResponse>> Restore([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            if (userId == Guid.Empty)
                return BadRequest("User ID cannot be empty or null");

            var result = await _mediator.Send(new RestoreCommand(userId), cancellationToken);

            if (result.RefreshToken != null)
                _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }

        [Authorize]
        [HttpPut("change/password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.CurrentPassword) || string.IsNullOrWhiteSpace(command.NewPassword))
                return BadRequest("Old password and new password cannot be empty");

            await _mediator.Send(command, cancellationToken);

            return Ok("Password changed successfully");
        }

        [Authorize]
        [HttpPut("email")]
        public async Task<ActionResult> AddEmail([FromBody] AddEmailCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Email))
                return BadRequest("Email cannot be empty.");

            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Email added successfully." });
        }

        [Authorize]
        [HttpPut("phone")]
        public async Task<ActionResult> AddPhone([FromBody] AddPhoneCommand command, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(command.Phone))
                return BadRequest("Phone number cannot be empty.");

            await _mediator.Send(command, cancellationToken);
            return Ok(new { Message = "Phone number added successfully." });
        }

        [Authorize]
        [HttpDelete("logout/all")]
        public async Task<ActionResult> LogoutAll(CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new LogoutFromAllDevicesCommand(), cancellationToken);
            _cookieService.Delete("refreshToken");
            return Ok("User logged out successfully");
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken = default)
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
                return BadRequest("Invalid request");

            await _mediator.Send(new LogoutFromDeviceCommand(refreshToken), cancellationToken);

            _cookieService.Delete("refreshToken");
            return Ok("User logged out successfully");
        }

        [HttpPost("token/refresh")]
        public async Task<ActionResult<AuthorizeResponse>> RefreshTokens(CancellationToken cancellationToken = default)
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
                return BadRequest("Invalid request");

            var result = await _mediator.Send(new RefreshCommand(refreshToken), cancellationToken);
            if (result == null)
                return Forbid("Invalid refresh token");

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }
    }
}
