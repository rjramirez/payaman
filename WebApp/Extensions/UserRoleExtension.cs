using Common.DataTransferObjects.ReferenceData;
using System.Security.Principal;
using WebApp.Services.Interfaces;

namespace WebApp.Extensions
{
    public static class UserRoleExtension
    {
        public async static Task<string> RoleName(this IPrincipal principal, ICommonService commonService)
        {
            ReferenceDataDetail roleDetail = await commonService.GetUserRole(principal);
            return roleDetail.Name;
        }

        public async static Task<byte> RoleId(this IPrincipal principal, ICommonService commonService)
        {
            ReferenceDataDetail roleDetail = await commonService.GetUserRole(principal);
            return byte.Parse(roleDetail.Name.ToString());
        }

        public async static Task<ReferenceDataDetail> UserRole(this IPrincipal principal, ICommonService commonService)
        {
            return await commonService.GetUserRole(principal);
        }
    }
}