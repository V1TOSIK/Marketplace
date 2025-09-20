using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.CurrentUser;

namespace SharedKernel.Authorization.Filters
{
    public class SameUserFilter : IAuthorizationFilter
    {
        private readonly ICurrentUserService _currentUserService;
        public SameUserFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var routeUserId = context.RouteData.Values["userId"]?.ToString();
            if (!Guid.TryParse(routeUserId, out var userId))
            {
                context.Result = new BadRequestObjectResult("Invalid user ID");
                return;
            }
            var currentUserId = _currentUserService.UserId;
            if (currentUserId != userId)
            {
                context.Result = new ForbidResult(); // 403
            }
        }
    }
}
