using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PhiThanh.Core.Filters;

namespace PhiThanh.Core.Extensions
{
    public static class EndpointRouteBuilderExtensions
    {
        public static RouteHandlerBuilder AddEndpoints<T>(this IEndpointRouteBuilder app, string route)
        {
            return app.MapPost(route, async (IMediator mediator, [FromBody] T request)
                => await mediator.Send(request!)).AddEndpointFilter<ValidationFilter<T>>();
        }

        public static RouteHandlerBuilder AddEndpointsWithFromForm<T>(this IEndpointRouteBuilder app, string route)
        {
            return app.MapPost(route, async (IMediator mediator, T request)
                => await mediator.Send(request!))
                .AddEndpointFilter<ValidationFilter<T>>()
                .Accepts<T>("multipart/form-data");
        }
    }
}
