using System.Linq;
using System.Security.Claims;

namespace manaslearning.Shared
{
    public static class ClaimsPrincipalHelper
    {
        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            var claims = ((ClaimsIdentity)claimsPrincipal.Identity).Claims.ToList();

            //return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            return claims.FirstOrDefault(c => c.Type == ClaimTypes.Upn)?.Value; // returns User Principal Name (Email)
        }
    }
}
