using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using AuthModule.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interfaces;
using System.Security.Claims;

namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
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
        public async Task<ActionResult<AuthorizeResponse>> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Register request is null");
                return BadRequest("Request cannot be null");
            }

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

            var clientInfo = new ClientInfo
            {
                IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
                Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
            };

            var result = await _authService.Register(request, clientInfo);

            if (result == null)
            {
                _logger.LogError("Registration failed.");
                return BadRequest("Registration failed.");
            }

            result.Response.AccessToken = _jwtProvider.GenerateAccessToken(result.Response.UserId, result.Response.Role);

            _cookieService.Set("refreshToken", result.RefreshToken.Token, result.RefreshToken.ExpirationDate);

            _logger.LogInformation("User registered successfully with ID: {UserId}", result.Response.UserId);
            return Ok(result.Response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthorizeResponse>> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Login request is null");
                return BadRequest("Request cannot be null");
            }

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

            var clientInfo = new ClientInfo
            {
                IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
                Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
            };

            var result = await _authService.Login(request, clientInfo);
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
        public async Task<ActionResult<AuthorizeResponse>> Restore([FromBody] RestoreRequest request)
        {
            if (request == null)
            {
                _logger.LogError("Restore request is null");
                return BadRequest("Request cannot be null");
            }
            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                _logger.LogError("Both email and phone number are empty in restore request");
                return BadRequest("Either email or phone number must be provided");
            }
            var clientInfo = new ClientInfo
            {
                IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
                Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
            };
            var result = await _authService.Restore(request, clientInfo);
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
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                _logger.LogError("Old password or new password is empty");
                return BadRequest("Old password and new password cannot be empty");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var parsedUserId))
            {
                _logger.LogError("Invalid user ID format: {UserId}", userId);
                return BadRequest("Invalid user ID format");
            }
            await _authService.ChangePassword(request, parsedUserId);

            _logger.LogInformation("Password changed successfully for user ID: {UserId}", userId);
            return Ok("Password changed successfully");
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<ActionResult> LogoutAll()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!Guid.TryParse(userIdString, out var userId))
            {
                _logger.LogError("Invalid user ID format: {UserId}", userIdString);
                return BadRequest("Invalid user ID format");
            }    

            await _authService.LogoutFromAllDevices(userId);

            _cookieService.Delete("refreshToken");

            _logger.LogInformation("User with ID: {UserId} logged out from all devices", userId);
            return Ok("User logged out successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token is null or empty");
                return BadRequest("Invalid request");
            }

            await _authService.LogoutFromDevice(refreshToken);

            _cookieService.Delete("refreshToken");
            return Ok("User logged out successfully");
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthorizeResponse>> RefreshTokens()
        {
            var refreshToken = _cookieService.Get("refreshToken");

            if (refreshToken == null)
            {
                _logger.LogError("Refresh token is null or empty");
                return BadRequest("Invalid request");
            }

            var clientInfo = new ClientInfo
            {
                IpAddress = HttpContext.Items["ClientIp"]?.ToString() ?? "unknown",
                Device = HttpContext.Items["ClientDevice"]?.ToString() ?? "unknown"
            };

            var result = await _authService.RefreshTokens(refreshToken, clientInfo);
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
    }
}
