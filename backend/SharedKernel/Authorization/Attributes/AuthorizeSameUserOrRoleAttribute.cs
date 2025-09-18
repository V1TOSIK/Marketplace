using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization.Filters;

namespace SharedKernel.Authorization.Attributes
{
    public class AuthorizeSameUserOrRoleAttribute : TypeFilterAttribute
    {
        public AuthorizeSameUserOrRoleAttribute(params string[] allowedRoles)
        : base(typeof(SameUserOrRoleFilter))
        {
            Arguments = new object[] { allowedRoles };
        }
    }
}
