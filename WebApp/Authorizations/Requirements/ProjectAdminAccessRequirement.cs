using Common.Constants;
using Common.DataTransferObjects.ProjectAdminAccess;
using Microsoft.AspNetCore.Authorization;
using System.Security.Principal;
using WebApp.Extensions;
using WebApp.Services.Interfaces;

namespace WebApp.Authorizations.Requirements
{
    public class ProjectAdminAccessRequirement : IAuthorizationRequirement
    {
        public async Task<bool> Pass(ICommonService commonService, IHttpContextAccessor httpContextAccessor)
        {
            IPrincipal principal = httpContextAccessor.HttpContext.User;

            if (principal.Identity.Name == null)
                return await Task.FromResult(false);

            IEnumerable<byte> roleIds = await principal.RoleIds(commonService);

            if (roleIds.Any(r => r == RoleConstant.SystemAdminRoledId))
            {
                return await Task.FromResult(true);
            }
            else if (roleIds.Any(r => r == RoleConstant.ProjectAdminRoledId))
            {
                IEnumerable<ProjectAdminAccessDetail> projectAdminAccessDetails = await principal.ProjectAdminAccess(commonService);

                IQueryCollection keyValuePairs = httpContextAccessor.HttpContext.Request.Query;
                string projectId = keyValuePairs["ProjectId"].ToString();
                string facilityId = keyValuePairs["FacilityId"].ToString();
                string departmentId = keyValuePairs["DepartmentId"].ToString();
                departmentId = string.IsNullOrEmpty(departmentId) ? null : departmentId;

                if (string.IsNullOrEmpty(projectId) && string.IsNullOrEmpty(facilityId))
                {
                    return await Task.FromResult(true);
                }
                else if (projectAdminAccessDetails.Any(
                    a => a.ProjectDetail.Value.ToString() == projectId &&
                    a.FacilityDetail.Value.ToString() == facilityId &&
                    a.DepartmentDetail.Value?.ToString() == departmentId))
                {
                    return await Task.FromResult(true);
                }
                else
                {
                    return await Task.FromResult(false);
                }
            }
            else
            {
                return await Task.FromResult(false);
            }
        }
    }
}
