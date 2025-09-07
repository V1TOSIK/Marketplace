using AuthModule.Application.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserModule.Application.Dtos;
using UserModule.Application.User.Commands.BanUser;
using UserModule.Application.User.Commands.CreateUser;
using UserModule.Application.User.Commands.DeactivateUser;
using UserModule.Application.User.Commands.DeleteUser;
using UserModule.Application.User.Commands.UnbanUser;
using UserModule.Application.User.Commands.UpdateUser;
using UserModule.Application.User.Queries.GetProfile;


namespace Marketplace.Api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ICookieService _cookieService;
        private readonly IMediator _mediator;

        public UserController(ICookieService cookieService,
            IMediator mediator)
        {
            _cookieService = cookieService;
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> MyProfile(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            var response = await _mediator.Send(new GetProfileQuery(userId), cancellationToken);

            return Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserProfile([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            var response = await _mediator.Send(new GetProfileQuery(userId), cancellationToken);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("me/profile")]
        public async Task<ActionResult> CreateProfile([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);
            return Ok("User profile successful ccreated");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{userId}/ban")]
        public async Task<ActionResult> BanUser([FromRoute] Guid userId, [FromBody] BanUserRequest request, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            await _mediator.Send(new BanUserCommand(userId, request.BanReason), cancellationToken);
            return Ok("User successful baned");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{userId}/ban")]
        public async Task<ActionResult> UnBanUser([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");
            await _mediator.Send(new UnbanUserCommand(userId), cancellationToken);
            return Ok("User successful unbaned");
        }

        [Authorize]
        [HttpPut("me/profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
        {
            await _mediator.Send(command, cancellationToken);

            return Ok("Account successful updated");
        }

        [Authorize]
        [HttpDelete("me/delete")]
        public async Task<ActionResult> HardDeleteAccount(CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Invalid user ID");

            await _mediator.Send(new DeleteUserCommand(userId), cancellationToken);
            _cookieService.Delete("refreshToken");

            return Ok("User successful deleted");
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

            return Ok("User successful deactivated");
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
