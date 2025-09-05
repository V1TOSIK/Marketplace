using AuthModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductModule.SharedKernel.Interfaces;
using SharedKernel.Interfaces;
using System.Security.Claims;
using UserModule.Application.Commands.BanUser;
using UserModule.Application.Commands.DeactivateUser;
using UserModule.Application.Commands.UnbanUser;
using UserModule.Application.Dtos;
using UserModule.Application.Dtos.Requests;
using UserModule.Application.Interfaces.Services;


namespace Marketplace.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ICookieService _cookieService;
        private readonly IUserBlockService _userBlockService;
        private readonly IMediator _mediator;

        public UserController(IUserService userService,
            ICookieService cookieService,
            IUserBlockService userBlockService,
            IMediator mediator)
        {
            _userService = userService;
            _cookieService = cookieService;
            _userBlockService = userBlockService;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> MyProfile(CancellationToken cancellationToken)
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
        public async Task<ActionResult<UserDto>> GetUserProfile(Guid userId, CancellationToken cancellationToken)
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
        public async Task<ActionResult<IEnumerable<UserDto>>> MyBlockedUsers(CancellationToken cancellationToken)
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
            await _mediator.Send(new BanUserCommand(userId), cancellationToken);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}/ban")]
        public async Task<ActionResult> UnBanUser(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            await _mediator.Send(new UnbanUserCommand(userId), cancellationToken);
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

            await _mediator.Send(new DeactivateUserCommand(userId), cancellationToken);
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
