using PhiThanh.Core;
using PhiThanh.Core.Extensions;
using PhiThanh.Core.Utils;
using System.Security.Claims;

namespace PhiThanh.Modules.AccountModule
{
    public class RefreshTokenRequest : BaseQuery<RefreshTokenResponse>
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAt { get; set; }
    }

    public class RefreshTokenProfileMapper : AutoMapper.Profile
    {
        public RefreshTokenProfileMapper()
        {
        }
    }

    public class RefreshTokenValidator : BaseValidator<RefreshTokenRequest>
    {
        public RefreshTokenValidator()
        {
        }
    }

    public class RefreshTokenHandler : IQueryHandler<RefreshTokenRequest, RefreshTokenResponse>
    {
        public async Task<ApiResponse<RefreshTokenResponse>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var principal = TokenUtils.GetPrincipalFromExpiredToken(request.AccessToken);
            bool isRefreshTokenValid = TokenUtils.ValidateExpiry(request.RefreshToken);

            if (isRefreshTokenValid && principal?.Identity is ClaimsIdentity identity)
            {
                DateTime? accessTokenExpires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(15));
                string accessToken = TokenUtils.GenerateAccessToken(identity.Claims, accessTokenExpires);

                DateTime? refreshTokenExpires = DateTime.UtcNow.Add(TimeSpan.FromDays(1));
                string refreshToken = TokenUtils.GenerateRefreshToken(refreshTokenExpires);

                return new ApiResponse<RefreshTokenResponse>(await Task.FromResult(new RefreshTokenResponse
                {
                    Email = identity.UserName(),
                    UserName = identity.UserName(),
                    AccessToken = accessToken,
                    AccessTokenExpireAt = accessTokenExpires,
                    RefreshToken = refreshToken,
                    RefreshTokenExpireAt = refreshTokenExpires
                }));
            }

            return new ApiResponse<RefreshTokenResponse>(new RefreshTokenResponse());
        }
    }
}
