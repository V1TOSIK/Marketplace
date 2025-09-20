using AuthModule.Application.Auth.Commands.SetEmail;
using AuthModule.Application.Auth.Commands.SetPhone;
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
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization.Attributes;
using SharedKernel.Authorization.Enums;
using AuthModule.Application.Auth.Commands.SetPassword;

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
        public async Task<ActionResult<AuthorizeResponse>> Restore([FromRoute] Guid userId, [FromBody] RestoreRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new RestoreCommand(userId, request), cancellationToken);

            if (result.RefreshToken != null)
                _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }

        [AuthorizeSameUser]
        [HttpPost("{userId}/password")]
        public async Task<ActionResult> SetPassword([FromRoute] Guid userId, [FromBody] SetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("New password cannot be empty");
            await _mediator.Send(new SetPasswordCommand(userId, request), cancellationToken);
            return Ok("Password set successfully");
        }

        [AuthorizeSameUser]
        [HttpPut("{userId}/password/change")]
        public async Task<ActionResult> ChangePassword([FromRoute] Guid userId, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest("Old password and new password cannot be empty");

            await _mediator.Send(new ChangePasswordCommand(userId, request), cancellationToken);

            return Ok("Password changed successfully");
        }

        [AuthorizeSameUser]
        [HttpPut("{userId}/email")]
        public async Task<ActionResult> SetEmail([FromRoute] Guid userId, [FromBody] SetEmailRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email cannot be empty.");

            await _mediator.Send(new SetEmailCommand(userId, request), cancellationToken);
            return Ok("Email added successfully.");
        }

        [AuthorizeSameUser]
        [HttpPut("{userId}/phone")]
        public async Task<ActionResult> SetPhone([FromRoute] Guid userId, [FromBody] SetPhoneRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Phone))
                return BadRequest("Phone number cannot be empty.");

            await _mediator.Send(new SetPhoneCommand(userId, request), cancellationToken);
            return Ok("Phone number added successfully.");
        }

        [AuthorizeSameUserOrRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator), nameof(AccessPolicy.SameUser))]
        [HttpDelete("{userId}/logout/all")]
        public async Task<ActionResult> LogoutAll([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new LogoutFromAllDevicesCommand(userId), cancellationToken);
            _cookieService.Delete("refreshToken");
            return Ok("User logged out successfully");
        }

        [AuthorizeSameUserOrRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator), nameof(AccessPolicy.SameUser))]
        [HttpDelete("{userId}/logout")]
        public async Task<ActionResult> Logout([FromRoute] Guid userId, CancellationToken cancellationToken = default)
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
                return BadRequest("Invalid request");

            await _mediator.Send(new LogoutFromDeviceCommand(userId, refreshToken), cancellationToken);

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
