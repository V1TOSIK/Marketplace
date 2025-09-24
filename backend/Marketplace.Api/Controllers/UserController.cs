using UserModule.Application.User.Commands.DeactivateUser;
using UserModule.Application.User.Commands.BanUser;
using UserModule.Application.User.Commands.UnbanUser;
using UserModule.Application.User.Commands.UpdateUser;
using UserModule.Application.Dtos;
using AuthModule.Application.Interfaces.Services;
using SharedKernel.Authorization.Attributes;
using SharedKernel.Authorization.Enums;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using UserModule.Application.User.Queries.GetUserProfile;
using UserModule.Application.User.Queries.GetMyProfile;


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

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDto>> GetUserProfile([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetUserProfileQuery(userId), cancellationToken);

            return Ok(response);
        }

        [AuthorizeSameUser]
        [HttpGet("{userId}/me")]
        public async Task<ActionResult<UserDto>> GetMyProfile([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetMyProfileQuery(userId), cancellationToken);

            return Ok(response);
        }

        [AuthorizeSameUser]
        [HttpPut("{userId}/profile")]
        public async Task<ActionResult> UpdateProfile([FromRoute] Guid userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UpdateUserCommand(userId, request), cancellationToken);

            return Ok("Account successful updated");
        }

        [AuthorizeRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator))]
        [HttpPut("{userId}/ban")]
        public async Task<ActionResult> BanUser([FromRoute] Guid userId, [FromBody] BanUserRequest request, CancellationToken cancellationToken)
        {
            await _mediator.Send(new BanUserCommand(userId, request), cancellationToken);

            return NoContent();
        }

        [AuthorizeRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator))]
        [HttpDelete("{userId}/ban")]
        public async Task<ActionResult> UnBanUser([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new UnbanUserCommand(userId), cancellationToken);

            return NoContent();
        }

        [AuthorizeSameUserOrRole(nameof(AccessPolicy.Admin), nameof(AccessPolicy.Moderator), nameof(AccessPolicy.SameUser))]
        [HttpPatch("{userId}/deactivate")]
        public async Task<ActionResult> SoftDeleteAccount([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            await _mediator.Send(new DeactivateUserCommand(userId), cancellationToken);

            _cookieService.Delete("refreshToken");

            return NoContent();
        }
    }
}
