using Microsoft.AspNetCore.Authorization;
using SharedKernel.Authorization.Enums;

namespace AuthModule.Infrastructure.Auth
{
    public class AccessRequirement : IAuthorizationRequirement
    {
        public AccessPolicy Policy { get; }
        public AccessRequirement(AccessPolicy policy)
        {
            Policy = policy;
        }
    }
}
