using PhiThanh.Core;
using PhiThanh.Core.Utils;
using PhiThanh.Resources;
using System.Security.Claims;

namespace PhiThanh.Modules.AccountModule
{
    public class AuthenticateRequest : BaseQuery<AuthenticateResponse>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class AuthenticateResponse
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public DateTime? AccessTokenExpireAt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? RefreshTokenExpireAt { get; set; }
    }

    public class AuthenticateProfileMapper : AutoMapper.Profile
    {
        public AuthenticateProfileMapper()
        {
        }
    }

    public class AuthenticateValidator : BaseValidator<AuthenticateRequest>
    {
        public AuthenticateValidator()
        {
        }
    }

    public class AuthenticateHandler() : IQueryHandler<AuthenticateRequest, AuthenticateResponse>
    {
        public async Task<ApiResponse<AuthenticateResponse>> Handle(AuthenticateRequest request, CancellationToken cancellationToken)
        {
            var rootUser = AppSettings.Instance.RootAccounts
                    .Find(f => string.Equals(f.UserName, request.UserName, StringComparison.OrdinalIgnoreCase)
                    && f.Password == request.Password);

            if (rootUser != null)
            {
                var claims = new List<Claim>
                    {
                        new(ClaimTypes.Sid, "0"),
                        new(ClaimTypes.PrimarySid, rootUser.UserName)
                    };

                DateTime? accessTokenExpires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(15));
                string accessToken = TokenUtils.GenerateAccessToken(claims, accessTokenExpires);

                DateTime? refreshTokenExpires = DateTime.UtcNow.Add(TimeSpan.FromDays(1));
                string refreshToken = TokenUtils.GenerateRefreshToken(refreshTokenExpires);

                return new ApiResponse<AuthenticateResponse>(await Task.FromResult(new AuthenticateResponse
                {
                    Email = rootUser.Email,
                    UserName = rootUser.UserName,
                    AccessToken = accessToken,
                    AccessTokenExpireAt = accessTokenExpires,
                    RefreshToken = refreshToken,
                    RefreshTokenExpireAt = refreshTokenExpires
                }));
            }
            else
            {
                ExceptionUtils.ThrowValidation(nameof(request.UserName),
                           nameof(ValidationMessage.ERR_001_USER_NOT_EXISTS),
                           ValidationMessage.ERR_001_USER_NOT_EXISTS);

                return new ApiResponse<AuthenticateResponse>(new AuthenticateResponse());
            }
        }
    }
}
