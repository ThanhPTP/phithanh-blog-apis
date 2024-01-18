using Microsoft.AspNetCore.Authorization;
using PhiThanh.Core.Attributes;

namespace PhiThanh.Core.Providers
{
    public class RolePermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(PermissionAttribute.Prefix, StringComparison.OrdinalIgnoreCase))
            {
                var permissionNames = policyName.Replace(PermissionAttribute.Prefix, "");
                return await Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(new PermissionRequirement(permissionNames)).Build());
            }

            if (policyName.StartsWith(RoleAttribute.Prefix, StringComparison.OrdinalIgnoreCase))
            {
                var roleNames = policyName.Replace(RoleAttribute.Prefix, "");
                return await Task.FromResult(new AuthorizationPolicyBuilder().AddRequirements(new RoleRequirement(roleNames)).Build());
            }

            return await GetDefaultPolicyAsync();
        }

        public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return await Task.FromResult(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());
        }

        public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        {
            return await Task.FromResult<AuthorizationPolicy>(null!);
        }
    }
}
