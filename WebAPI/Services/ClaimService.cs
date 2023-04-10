using Common.Constants;
using System.Security.Claims;
using System.Security.Principal;

namespace WebAPI.Services
{
    public static class ClaimService
    {
        public static string GetClaimStringValue(IPrincipal User, string claimKey)
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity.Claims.FirstOrDefault(i => i.Type == claimKey).Value;
        }

        public static string GetClientId(IPrincipal User)
        {
            var identity = User.Identity as ClaimsIdentity;

            if (identity.IsAuthenticated)
            {
                return identity.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;
            }
            else
            {
                return "Anonymous";
            }
        }
    }
}
