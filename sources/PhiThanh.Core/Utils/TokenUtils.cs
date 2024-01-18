using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhiThanh.Core.Utils
{
    public static class TokenUtils
    {
        public static string GenerateAccessToken(IEnumerable<Claim> claims, DateTime? accessTokenExpires)
        {
            JwtSecurityToken token = new(issuer: AppSettings.Instance.Issuer,
                audience: AppSettings.Instance.Audience,
                claims: claims,
                notBefore: null,
                expires: accessTokenExpires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.TokenSecretKey)), "HS256"));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken(DateTime? refreshTokenExpires)
        {
            string value = "";
            byte[] array = new byte[32];
            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(array);
                value = Convert.ToBase64String(array);
            }

            List<Claim> claims =
            [
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash", value)
            ];
            JwtSecurityToken token = new(issuer: null,
                audience: null,
                claims: claims,
                notBefore: null,
                expires: refreshTokenExpires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.TokenSecretKey)), "HS256"));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters validationParameters = new()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = AppSettings.Instance.Issuer,
                ValidAudience = AppSettings.Instance.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.TokenSecretKey)),
                ValidateLifetime = false
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            SecurityToken? validatedToken;
            ClaimsPrincipal? result;
            try
            {
                result = jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out validatedToken);
            }
            catch (ArgumentException)
            {
                result = null;
                validatedToken = null;
            }

            if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals("HS256", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ValidationException("Invalid token");
            }

            return result;
        }

        public static bool ValidateExpiry(string token)
        {
            TokenValidationParameters validationParameters = new()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.TokenSecretKey)),
                ValidateLifetime = true
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            jwtSecurityTokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            return validatedToken != null;
        }
    }
}
