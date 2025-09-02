using Microsoft.AspNetCore.Http;
using ProductModule.SharedKernel.Interfaces;
using System.Security.Claims;

namespace AuthModule.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Guid? UserId
            => Guid.TryParse(
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var userId
            ) ? userId : null;
    }
}
