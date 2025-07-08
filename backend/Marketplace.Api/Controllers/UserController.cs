using AuthModule.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;
using UserModule.Application.Interfaces;


namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserResponse>> MyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();

            var response = await _userService.GetProfile(parsedUserId);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("my-blocked")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> MyBlockedUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();

            var blockedUsers = await _userService.GetBlockedUsers(parsedUserId);

            if (blockedUsers == null || !blockedUsers.Any())
                return NotFound();
            return Ok(blockedUsers);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponse>> GetUserAccount(Guid userId)
        {
            if (userId ==  Guid.Empty)
                return BadRequest();

            var response = await _userService.GetProfile(userId);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] CreateUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();

            if (request == null)
                return BadRequest();

            await _userService.CreateNewProfile(parsedUserId, request);
            return Ok();
        }

        [Authorize]
        [HttpPost("block/{userId}")]
        public async Task<ActionResult> BlockUser(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid parsedCurrentUserId))
                return BadRequest();
            if (userId == Guid.Empty || userId == parsedCurrentUserId)
                return BadRequest("Invalid user ID");
            await _userService.BlockUser(parsedCurrentUserId, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("unblock/{userId}")]
        public async Task<ActionResult> UnblockUser(Guid userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(currentUserId, out Guid parsedCurrentUserId))
                return BadRequest();
            if (userId == Guid.Empty || userId == parsedCurrentUserId)
                return BadRequest("Invalid user ID");
            await _userService.UnblockUser(parsedCurrentUserId, userId);
            return Ok();
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult> UpdateAccountInfo([FromBody] UpdateUserRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();

            if (request == null)
                return BadRequest();

            await _userService.UpdateProfile(parsedUserId, request);
            return Ok();
        }

        [Authorize]
        [HttpDelete("hard-delete")]
        public async Task<ActionResult> HardDeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();
            await _userService.HardDeleteProfile(parsedUserId);
            return Ok();
        }

        [Authorize]
        [HttpDelete("soft-delete")]
        public async Task<ActionResult> SoftDeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out Guid parsedUserId))
                return BadRequest();
            await _userService.SoftDeleteProfile(parsedUserId);
            return Ok();
        }
    }
}
