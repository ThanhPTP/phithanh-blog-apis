using Microsoft.AspNetCore.Authorization;

namespace PhiThanh.Core.Providers
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy = new AuthorizationPolicyBuilder();

            if (!string.IsNullOrWhiteSpace(policyName))
            {
                policy.AddRequirements(new PermissionRequirement(policyName));
                return await Task.FromResult(policy.Build());
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
