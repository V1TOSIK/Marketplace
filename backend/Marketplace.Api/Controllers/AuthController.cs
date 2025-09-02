using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interfaces;
using System.Security.Claims;
using UserModule.Domain.Entities;

namespace Marketplace.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtProvider _jwtProvider;
        private readonly ICookieService _cookieService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(
            IAuthService authService,
            IJwtProvider jwtProvider,
            ICookieService cookieService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtProvider = jwtProvider;
            _cookieService = cookieService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthorizeResponse>> Register([FromBody] RegisterLocalRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogError("Password is empty or null in register request");
                return BadRequest("Password cannot be empty or null");
            }

            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                _logger.LogError("Both email and phone number are empty in register request");
                return BadRequest("Either email or phone number must be provided");
            }

            var clientInfo = GetClientInfo();

            var result = await _authService.RegisterLocalUser(request, clientInfo, cancellationToken);

            if (result == null)
            {
                _logger.LogError("Registration failed.");
                return BadRequest("Registration failed.");
            }

            result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            return Ok(result.Response);
        }

        [HttpPost("local/login")]
        public async Task<ActionResult<AuthorizeResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogError("Password is empty or null in login request");
                return BadRequest("Password cannot be empty or null");
            }

            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                _logger.LogError("Both email and phone number are empty in login request");
                return BadRequest("Either email or phone number must be provided");
            }

            var clientInfo = GetClientInfo();

            var result = await _authService.Login(request, clientInfo, cancellationToken);
            if (result == null)
            {
                _logger.LogError("Login failed. Invalid credentials.");
                return Unauthorized("Invalid credentials");
            }

            result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            _logger.LogInformation("User logged in successfully with ID: {UserId}", result.Response.UserId);
            return Ok(result.Response);
        }

        [HttpPost("restore")]
        public async Task<ActionResult<AuthorizeResponse>> Restore([FromBody] RestoreRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                _logger.LogError("Both email and phone number are empty in restore request");
                return BadRequest("Either email or phone number must be provided");
            }
            var clientInfo = GetClientInfo();
            var result = await _authService.Restore(request, clientInfo, cancellationToken);
            if (result == null)
            {
                _logger.LogError("Restore failed. Invalid credentials.");
                return Unauthorized("Invalid credentials");
            }
            result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);
            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);
            _logger.LogInformation("User restored successfully with ID: {UserId}", result.Response.UserId);
            return Ok(result.Response);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                _logger.LogError("Old password or new password is empty");
                return BadRequest("Old password and new password cannot be empty");
            }
            var userId = GetUserId();
            if (userId == Guid.Empty)
            {
                _logger.LogError("User ID is null or invalid");
                return BadRequest("Invalid user ID");
            }
            await _authService.ChangePassword(request, userId, cancellationToken);

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);
            return Ok("Password changed successfully");
        }

        [Authorize]
        [HttpPost("email")]
        public async Task<ActionResult> AddEmail([FromBody] AddEmailRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
            {
                _logger.LogError("User ID is null or invalid");
                return BadRequest("Invalid user ID");
            }

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email cannot be empty.");

            await _authService.AddEmailAsync(userId, request.Email, cancellationToken);
            return Ok(new { Message = "Email added successfully." });
        }

        [Authorize]
        [HttpPost("phone")]
        public async Task<ActionResult> AddPhone([FromBody] AddPhoneRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
            {
                _logger.LogError("User ID is null or invalid");
                return BadRequest("Invalid user ID");
            }

            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return BadRequest("Phone number cannot be empty.");

            await _authService.AddPhoneAsync(userId, request.PhoneNumber, cancellationToken);
            return Ok(new { Message = "Phone number added successfully." });
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<ActionResult> LogoutAll(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
            {
                _logger.LogError("User ID is null or invalid");
                return BadRequest("Invalid user ID");
            }

            await _authService.LogoutFromAllDevices(userId, cancellationToken);

            _cookieService.Delete("refreshToken");

            _logger.LogInformation("User with ID: {UserId} logged out from all devices", userId);
            return Ok("User logged out successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token is null or empty");
                return BadRequest("Invalid request");
            }

            await _authService.LogoutFromDevice(refreshToken, cancellationToken);

            _cookieService.Delete("refreshToken");
            return Ok("User logged out successfully");
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthorizeResponse>> RefreshTokens(CancellationToken cancellationToken)
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token is null or empty");
                return BadRequest("Invalid request");
            }

            var clientInfo = GetClientInfo();

            var result = await _authService.RefreshTokens(refreshToken, clientInfo, cancellationToken);
            if (result == null)
            {
                _logger.LogError("Refresh token is invalid or expired");
                return Forbid("Invalid refresh token");
            }

            result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            _logger.LogInformation("Tokens refreshed successfully for user ID: {UserId}", result.Response.UserId);
            return Ok(result.Response);
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out var parsedUserId))
                return parsedUserId;
            return Guid.Empty;
        }

        private ClientInfo GetClientInfo()
        {
            return new ClientInfo
            {
                IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
                Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
            };
        }

    }
}
