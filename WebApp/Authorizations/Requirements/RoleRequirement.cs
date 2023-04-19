using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using WebApp.Extensions;
using WebApp.Services.Interfaces;

namespace WebApp.Authorizations.Requirements
{
    public class RoleRequirement : IAuthorizationRequirement
    {
        private readonly byte[] _roleIds;
        public RoleRequirement(byte[] roleIds)
        {
            _roleIds = roleIds;
        }

        public async Task<bool> Pass(ICommonService commonService, IPrincipal User)
        {
            if (User.Identity.Name == null)
                return await Task.FromResult(false);

            ReferenceDataDetail userRole = await User.UserRole(commonService);

            if (_roleIds.Contains(byte.Parse(userRole.Value.ToString())))
            {
                return await Task.FromResult(true);
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
