using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PhiThanh.Core.Utils;

namespace PhiThanh.Core.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHttpContext(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();
            HttpContextUtils.Configure(httpContextAccessor);
            return app;
        }
    }
}
