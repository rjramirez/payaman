using Common.Constants;
using System.Security.Claims;
using System.Security.Principal;

namespace WebApp.Services
{
    public static class ClaimService
    {
        public static string GetClaimStringValue(IPrincipal User, string claimKey)
        {
            var identity = User.Identity as ClaimsIdentity;
            return identity.Claims.FirstOrDefault(i => i.Type == claimKey).Value;
        }
    }
}
