using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.CurrentUser;

namespace SharedKernel.Authorization.Filters
{
    public class SameUserOrRoleFilter : IAuthorizationFilter
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly string[] _allowedRoles;

        public SameUserOrRoleFilter(ICurrentUserService currentUserService,
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
            var currentUserId = _currentUserService.UserId;

            if (!_allowedRoles.Contains(role) && currentUserId != userId)
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }
}
