using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PhiThanh.Core.Utils
{
    public static class HttpContextUtils
    {
        private static IHttpContextAccessor? _contextAccessor;

        public static HttpContext? Current => _contextAccessor?.HttpContext;
        public static ClaimsIdentity? Identity => Current?.User.Identity as ClaimsIdentity;
        public static void Configure(IHttpContextAccessor? contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
}
