using Microsoft.AspNetCore.Authorization;

namespace PhiThanh.Core.Attributes
{
    public class RoleAttribute : AuthorizeAttribute
    {
        public const string Prefix = "PHITHANH.ROLE:";
        public RoleAttribute(params string[] roles)
        {
            Policy = Prefix + string.Join(',', roles);
        }
    }
}
