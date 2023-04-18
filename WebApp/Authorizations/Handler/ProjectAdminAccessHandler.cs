using Microsoft.AspNetCore.Authorization;
using WebApp.Authorizations.Requirements;
using WebApp.Services.Interfaces;

namespace WebApp.Authorizations.Handler
{
    public class ProjectAdminAccessHandler : AuthorizationHandler<ProjectAdminAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonService _commonService;

        public ProjectAdminAccessHandler(ICommonService commonService, IHttpContextAccessor httpContextAccessor)
        {
            _commonService = commonService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProjectAdminAccessRequirement requirement)
        {
            if (context.User == null)
                context.Fail();

            if (await requirement.Pass(_commonService, _httpContextAccessor))
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
