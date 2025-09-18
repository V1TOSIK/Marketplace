using AuthModule.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using SharedKernel.Authorization.Enums;
using SharedKernel.Interfaces;

namespace AuthModule.Infrastructure.Auth
{
    public class AccessHandler : AuthorizationHandler<AccessRequirement, Guid>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<AccessHandler> _logger;
        public AccessHandler(ICurrentUserService currentUserService, ILogger<AccessHandler> logger)
        {
            _currentUserService = currentUserService;
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            AccessRequirement requirement,
            Guid resource)
        {
            var role = _currentUserService.Role?.ToLower();
            _logger.LogInformation("User role: {Role}, Required policy: {Policy}, Resource ID: {ResourceId}", role, requirement.Policy, resource);
            var userId = _currentUserService.UserId;

            var admin = UserRole.Admin.ToString().ToLower();
            var moderator = UserRole.Moderator.ToString().ToLower();
            var user = UserRole.User.ToString().ToLower();

            switch (requirement.Policy)
            {
                case AccessPolicy.Admin:
                    if (role == admin)
                        context.Succeed(requirement);
                    break;
                case AccessPolicy.Moderator:
                    if (role == moderator)
                        context.Succeed(requirement);
                    break;
                case AccessPolicy.SameUser:
                    if (userId.HasValue && userId.Value == resource)
                        context.Succeed(requirement);
                    break;
                default:
                    context.Fail();
                    break;
            }
            
            return Task.CompletedTask;
        }
    }
}
