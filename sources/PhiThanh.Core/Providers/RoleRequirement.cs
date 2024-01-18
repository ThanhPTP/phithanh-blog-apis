using Microsoft.AspNetCore.Authorization;

namespace PhiThanh.Core.Providers
{
    public class RoleRequirement(string roles) : IAuthorizationRequirement
    {
        public const string PolicyPrefix = "PHITHANH.ROLE:";

        public IEnumerable<string> Roles { get; } = (roles ?? "").Split(',');
    }
}
