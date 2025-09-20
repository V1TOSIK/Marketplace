using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SharedKernel.CurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? Device
            => _httpContextAccessor.HttpContext?.Items["ClientDevice"]?.ToString() ?? "unknown";

        public string? IpAddress
            => _httpContextAccessor.HttpContext?.Items["ClientIp"]?.ToString() ?? "unknown";

        public string? Role
            => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public Guid? UserId
            => Guid.TryParse(
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var userId
            ) ? userId : null;
    }
}
