using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserModule.Application.Interfaces;


namespace Marketplace.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        public UserController()
        {
            
        }
        [HttpGet("{userId}")]
        public async Task<ActionResult> GetUser(Guid userId)
        {
            // Simulate fetching user data
            await Task.Delay(100); // Simulating async operation
            return Ok(new { UserId = userId, Name = "John Doe" });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetMyAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Simulate fetching current user's account data
            await Task.Delay(100); // Simulating async operation
            return Ok(new { UserId = Guid.NewGuid(), Name = "Jane Doe" });
        }
    }
}
