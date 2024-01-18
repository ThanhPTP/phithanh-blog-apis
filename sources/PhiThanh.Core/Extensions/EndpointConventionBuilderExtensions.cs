using Microsoft.AspNetCore.Builder;
using PhiThanh.Core.Attributes;

namespace PhiThanh.Core.Extensions
{
    public static class EndpointConventionBuilderExtensions
    {
        public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, params string[] permissions)
        where TBuilder : IEndpointConventionBuilder
        {
            builder.Add(endpointBuilder =>
            {
                endpointBuilder.Metadata.Add(
                new PermissionAttribute(permissions));
            });
            return builder;
        }
    }
}
