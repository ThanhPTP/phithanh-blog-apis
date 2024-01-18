using Microsoft.AspNetCore.Authorization;

namespace PhiThanh.Core.Providers
{
    public class PermissionRequirement(string permissions) : IAuthorizationRequirement
    {
        public IEnumerable<string> Permissions { get; } = (permissions ?? "").Split(',');
    }
}