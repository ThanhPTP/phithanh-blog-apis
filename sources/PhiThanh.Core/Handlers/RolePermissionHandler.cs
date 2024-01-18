using Microsoft.AspNetCore.Authorization;
using PhiThanh.Core.Providers;
using System.Security.Claims;

namespace PhiThanh.Core.Handlers
{
    public class RolePermissionHandler : IAuthorizationHandler
    {
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            var result = true;
            foreach (var requirement in context.PendingRequirements)
            {
                if (requirement is RoleRequirement roleRequirement)
                {
                    if (!CheckRoles(context.User, roleRequirement.Roles))
                    {
                        result = false;
                        break;
                    }

                    context.Succeed(requirement);
                }

                if (requirement is PermissionRequirement permissionRequirement)
                {
                    if (!CheckPermissions(context.User, permissionRequirement.Permissions))
                    {
                        result = false;
                        break;
                    }

                    context.Succeed(requirement);
                }
            }

            if (!result)
            {
                context.Fail();
            }

            await Task.CompletedTask;
        }

        protected static bool CheckPermissions(ClaimsPrincipal user, IEnumerable<string> requirePermissions)
        {
            if (user.Identity!.IsAuthenticated)
            {
                var permissions = requirePermissions?.ToList() ?? [];
                if (user.Claims.Any(c => c.Type == "Permission"
                                        && permissions.Any(p => p == c.Value)))
                {
                    return true;
                }
            }

            return false;
        }

        protected static bool CheckRoles(ClaimsPrincipal user, IEnumerable<string> requireRoles)
        {
            if (user.Identity!.IsAuthenticated)
            {
                var permissions = requireRoles?.ToList() ?? [];
                if (user.Claims.Any(c => c.Type == "Role"
                                        && permissions.Any(p => p == c.Value)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
