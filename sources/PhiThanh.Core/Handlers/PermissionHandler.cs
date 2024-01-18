using Microsoft.AspNetCore.Authorization;
using PhiThanh.Core.Providers;

namespace PhiThanh.Core.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User.Claims.Any(claim =>
                    claim.Type == "Permission" &&
                    requirement.Permissions.Any(permission => permission == claim.Value)))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
