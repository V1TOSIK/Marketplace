using AuthModule.Application.Dtos.Requests;
using AuthModule.Application.Dtos.Responses;
using AuthModule.Application.Interfaces;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Login([FromBody] AuthorizeUserRequest request)
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
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
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

            return CreatedAtAction(nameof(Login), new { id = response.UserId }, response);
        }

        [HttpPost("logout/{userId}")]
        public async Task<IActionResult> Logout([FromRoute] Guid userId)
        {
            if (await _authService.Logout(userId))
                return Ok("User logged out successfully");

            return BadRequest("User cannot logged out");
        }
    }
}
