using Microsoft.AspNetCore.Authorization;
using WebApp.Authorizations.Requirements;
using WebApp.Services.Interfaces;

namespace WebApp.Authorizations.Handler
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommonService _commonService;

        public RoleHandler(ICommonService commonService, IHttpContextAccessor httpContextAccessor)
        {
            _commonService = commonService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            if (context.User == null)
                context.Fail();

            if (await requirement.Pass(_commonService, _httpContextAccessor.HttpContext.User))
                context.Succeed(requirement);
            else
                context.Fail();
        }
    }
}
