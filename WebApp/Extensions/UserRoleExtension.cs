using Common.Constants;
using Common.DataTransferObjects.AppSettings;
using System.Security.Claims;
using System.Security.Principal;

namespace WebApp.Extensions
{
    public static class UserRoleExtension
    {
        public static List<string> ApplicationRoleName(this IPrincipal User)
        {
            //TODO: Customize Application role
            IConfiguration configuration = StaticConfiguration.Configuration;
            List<string> applicationRole = new();
            SecurityGroup securityGroup = new();
            configuration.Bind("SecurityGroup", securityGroup);

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> groupClaims = claimsIdentity.Claims.Where(c => c.Type == "groups").ToList();

            if (groupClaims.Any(c => securityGroup.ApplicationSupport.Contains(c.Value)))
            {
                applicationRole.Add(RoleConstant.Support);
            }

            return applicationRole;
        }
    }
}
