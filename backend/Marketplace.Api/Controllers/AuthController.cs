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
        public AuthController(
            IAuthService authService,
            IJwtProvider jwtProvider)
        {
            _authService = authService;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthorizeResponse>> Login([FromBody] LoginRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password cannot be empty or null");
            }

            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest("Either email or phone number must be provided");
            }

            var response = await _authService.Login(request);
            if (response == null)
            {
                return Unauthorized("Invalid credentials");
            }

            response.AccessToken = _jwtProvider.GenerateAccessToken(response.UserId, response.Role);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthorizeResponse>> Register([FromBody] RegisterRequest request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password cannot be empty or null");
            }

            if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                return BadRequest("Either email or phone number must be provided");
            }

            var response = await _authService.Register(request);

            if (response == null)
            {
                return BadRequest("Registration failed. User already exists.");
            }

            response.AccessToken = _jwtProvider.GenerateAccessToken(response.UserId, response.Role);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout-all")]
        public async Task<ActionResult> Logout()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (!Guid.TryParse(userIdString, out var userId))
                return BadRequest("Invalid user ID format");

            await _authService.LogoutFromAllDevices(userId);
            
            return Ok("User logged out successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Invalid request");
            }

            await _authService.LogoutFromDevice(request.RefreshToken);
            
            return Ok("User logged out successfully");
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthorizeResponse>> RefreshTokens([FromBody] RefreshTokensRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return BadRequest("Invalid request");
            }

            var response = await _authService.RefreshTokens(request.RefreshToken);
            if (response == null)
            {
                return Forbid("Invalid refresh token");
            }

            response.AccessToken = _jwtProvider.GenerateAccessToken(response.UserId, response.Role);

            return Ok(response);
        }
    }
}
