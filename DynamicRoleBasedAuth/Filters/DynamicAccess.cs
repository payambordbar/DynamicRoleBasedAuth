using DynamicRoleBasedAuth.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DynamicRoleBasedAuth.Filters;

/// <summary>
/// Checks User claims for entire controller or spesific action
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DynamicAccessAttribute : TypeFilterAttribute
{
    public DynamicAccessAttribute() : base(typeof(AccessControlImplementation))
    {
    }

    private class AccessControlImplementation : Attribute, IAuthorizationFilter
    {
        private readonly ActionDetection _actionDetection;

        public AccessControlImplementation(ActionDetection actionDetection)
        {
            _actionDetection = actionDetection;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var href = context.HttpContext.Request.Path;
            var action = _actionDetection.ParsePath(href, context.HttpContext.Request.Method);
            var hasClaim = context.HttpContext.User.HasClaim(AppClaimTypes.Permission, action.Claim);

            if (!(context.HttpContext.User.IsInRole("Admin") || hasClaim))
            {
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}
