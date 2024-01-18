using Carter;
using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Modules.AccountModule;

namespace PhiThanh.WebApi.Endpoints
{
    public class AccountEndpoints : CarterModule
    {
        private const string TAG_ROUTE = "account";
        public AccountEndpoints() : base($"api/v{{version:apiVersion}}/{TAG_ROUTE}")
        {
            WithTags(TAG_ROUTE);
            RequireAuthorization();
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            app.AddEndpoints<AuthenticateRequest>("/authenticate")
                .Produces(200, typeof(ApiResponse<AuthenticateResponse>))
                .AllowAnonymous();

            app.AddEndpoints<RefreshTokenRequest>("/refresh-token")
                .Produces(200, typeof(ApiResponse<AuthenticateResponse>))
                .AllowAnonymous();

            app.AddEndpoints<GetProfileRequest>("/get-profile")
                .Produces(200, typeof(ApiResponse<GetProfileResponse>));
        }
    }
}
