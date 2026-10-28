using manaslearning.Models;
using Microsoft.AspNetCore.Http;

namespace manaslearning.UI.Helpers
{
    public static class IdentityHelper
    {
        public static long GetUserId(this HttpContext context)
        {
            try
            {
                var claim = context?.User?.FindFirst(c => c.Type == CustomClaimTypes.UserId);
                if (claim == null)
                    return 0;

                if (long.TryParse(claim.Value, out long userId))
                    return userId;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static int GetOrganizationId(this HttpContext context)
        {
            try
            {
                var claim = context?.User?.FindFirst(c => c.Type == CustomClaimTypes.OrganizationId);
                if (claim == null)
                    return 0;

                if (int.TryParse(claim.Value, out int organizationId))
                    return organizationId;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        public static Enums.LoginTypes GetLoginType(this HttpContext context)
        {
            try
            {
                var claim = context?.User?.FindFirst(c => c.Type == CustomClaimTypes.LoginType);
                if (claim == null)
                    return Enums.LoginTypes.None;

                if (int.TryParse(claim.Value, out int loginType))
                    return (Enums.LoginTypes)loginType;
                else
                    return Enums.LoginTypes.None;
            }
            catch
            {
                return Enums.LoginTypes.None;
            }
        }

        public static string GetClaim(this HttpContext context, string claimType)
        {
            try
            {
                var claim = context?.User?.FindFirst(c => c.Type == claimType);
                if (claim == null)
                    return null;

                return claim.Value;
            }
            catch
            {
                return null;
            }
        }

    }
}
