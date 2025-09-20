using Microsoft.AspNetCore.Mvc;
using SharedKernel.Authorization.Filters;

namespace SharedKernel.Authorization.Attributes
{
    public class AuthorizeSameUserAttribute : TypeFilterAttribute
    {
        public AuthorizeSameUserAttribute()
            : base(typeof(SameUserFilter))
        {
        }
    }
}
