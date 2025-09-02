using AuthModule.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Interfaces;
using System.Security.Claims;
using UserModule.Application.Dtos.Requests;
using UserModule.Application.Dtos.Responses;
using UserModule.Application.Interfaces;


namespace Marketplace.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ICookieService _cookieService;
        private readonly IUserBlockService _userBlockService;
        public UserController(IUserService userService,
            ICookieService cookieService,
            IUserBlockService userBlockService)
        {
            _userService = userService;
            _cookieService = cookieService;
            _userBlockService = userBlockService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> MyProfile(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            var response = await _userService.GetProfile(userId, cancellationToken);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponse>> GetUserProfile(Guid userId, CancellationToken cancellationToken)
        {
            if (userId ==  Guid.Empty)
                return BadRequest();

            var response = await _userService.GetProfile(userId, cancellationToken);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [Authorize]
        [HttpGet("me/blocked")]
        public async Task<ActionResult<IEnumerable<UserResponse>>> MyBlockedUsers(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            var blockedUsers = await _userBlockService.GetBlockedUsers(userId, cancellationToken);

            return Ok(blockedUsers);
        }

        [Authorize]
        [HttpPost("me/profile")]
        public async Task<ActionResult> CreateProfile([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            await _userService.CreateNewProfile(userId, request, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/ban")]
        public async Task<ActionResult> BanUser(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            await _userService.BanProfile(userId, cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}/ban")]
        public async Task<ActionResult> UnBanUser(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            await _userService.UnBanProfile(userId, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpPost("{userId}/block")]
        public async Task<ActionResult> BlockUser(Guid userId, CancellationToken cancellationToken)
        {
            var currentUserId = GetUserId();
            if (currentUserId == Guid.Empty)
                return BadRequest("Invalid user ID");

            if (userId == Guid.Empty || userId == currentUserId)
                return BadRequest("Invalid user ID");

            await _userBlockService.BlockUser(currentUserId, userId, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpDelete("{userId}/block")]
        public async Task<ActionResult> UnblockUser(Guid userId, CancellationToken cancellationToken)
        {
            var currentUserId = GetUserId();
            if (currentUserId == Guid.Empty)
                return BadRequest("Invalid user ID");

            if (userId == Guid.Empty || userId == currentUserId)
                return BadRequest("Invalid user ID");

            await _userBlockService.UnblockUser(currentUserId, userId, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpPut("me/profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            await _userService.UpdateProfile(userId, request, cancellationToken);
            return Ok();
        }

        [Authorize]
        [HttpDelete("me/delete")]
        public async Task<ActionResult> HardDeleteAccount(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            await _userService.HardDeleteProfile(userId, cancellationToken);
            _cookieService.Delete("refreshToken");

            return Ok();
        }

        [Authorize]
        [HttpPatch("me/deactivate")]
        public async Task<ActionResult> SoftDeleteAccount(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            await _userService.SoftDeleteProfile(userId, cancellationToken);
            _cookieService.Delete("refreshToken");

            return Ok();
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userId, out Guid parsedUserId))
                return parsedUserId;

            return Guid.Empty;
        }
    }
}
