using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhiThanh.Core.Handlers;
using PhiThanh.Core.Providers;
using PhiThanh.Core.Services;
using System.Configuration;

namespace PhiThanh.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpContext(this IServiceCollection services)
        {
            if (services.All(x => x.ServiceType != typeof(IHttpContextAccessor)))
            {
                services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            }

            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            return services;
        }

        public static IServiceCollection AddPermissionAuthorize(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            return services;
        }

        public static IServiceCollection AddRolePermissionAuthorize(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, RolePermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, RolePermissionHandler>();

            return services;
        }

        public static IServiceCollection AddMinioBlobStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection("MinioBlobStorage").Get<MinioOptions>();
            services.AddSingleton(options!);
            services.AddScoped<IFileStorage, MinioObjectStorage>();
            return services;
        }
    }
}
