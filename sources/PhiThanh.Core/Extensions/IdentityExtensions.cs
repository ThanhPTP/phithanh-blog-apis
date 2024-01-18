using System.Security.Claims;

namespace PhiThanh.Core.Extensions
{
    public static class IdentityExtensions
    {
        public static Guid UserId(this ClaimsIdentity claimsIdentity)
        {
            return Guid.Parse(GetClaimValue(claimsIdentity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/sid"));
        }

        public static string UserName(this ClaimsIdentity claimsIdentity)
        {
            return GetClaimValue(claimsIdentity, "http://schemas.microsoft.com/ws/2008/06/identity/claims/primarysid");
        }

        private static string GetClaimValue(ClaimsIdentity claimsIdentity, string claimType)
        {
            Claim? claim = claimsIdentity.FindFirst(claimType);
            if (claim != null)
            {
                return claim.Value;
            }

            return string.Empty;
        }
    }
}
