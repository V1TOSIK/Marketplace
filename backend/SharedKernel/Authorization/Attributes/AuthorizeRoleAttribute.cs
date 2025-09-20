using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization.Filters;

namespace SharedKernel.Authorization.Attributes
{
    public class AuthorizeRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeRoleAttribute(params string[] allowedRoles)
        : base(typeof(AuthorizeRoleFilter))
        {
            Arguments = new object[] { allowedRoles };
        }
    }
}
