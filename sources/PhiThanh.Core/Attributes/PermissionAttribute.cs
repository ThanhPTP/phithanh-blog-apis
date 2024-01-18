using Microsoft.AspNetCore.Authorization;

namespace PhiThanh.Core.Attributes
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        public const string Prefix = "PHITHANH.PERMISSION:";
        public PermissionAttribute(params string[] permissions)
        {
            Policy = Prefix + string.Join(',', permissions);
        }
    }
}
