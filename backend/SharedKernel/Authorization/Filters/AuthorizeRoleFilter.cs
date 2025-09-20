using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.CurrentUser;

namespace SharedKernel.Authorization.Filters
{
    public class AuthorizeRoleFilter : IAuthorizationFilter
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly string[] _allowedRoles;
        public AuthorizeRoleFilter(ICurrentUserService currentUserService,
            params string[] allowedRoles)
        {
            _currentUserService = currentUserService;
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var routeUserId = context.RouteData.Values["userId"]?.ToString();
            if (!Guid.TryParse(routeUserId, out var userId))
            {
                context.Result = new BadRequestObjectResult("Invalid user ID");
                return;
            }

            var role = _currentUserService.Role;

            if (!_allowedRoles.Contains(role))
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }
}
