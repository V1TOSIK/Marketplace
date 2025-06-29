using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

            var result = await _authService.Register(request);

            if (result == null)
            {
                _logger.LogError("Registration failed. User already exists.");
                return BadRequest("Registration failed. User already exists.");
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

            var result = await _authService.Login(request);
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

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

            var result = await _authService.RefreshTokens(refreshToken);
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
