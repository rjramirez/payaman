using Common.DataTransferObjects.ReferenceData;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "SystemData")]
    [ApiVersion("1.0")]
    public class ReferenceDataController : Controller
    {
        private readonly IRITSDBUnitOfWork _ritsDBUnitOfWork;
        public ReferenceDataController(IRITSDBUnitOfWork ritsDBUnitOfWork)
        {
            _ritsDBUnitOfWork = ritsDBUnitOfWork;
        }


        [HttpGet]
        [Route("AppUserRoles")]
        [SwaggerOperation(Summary = "Get AppUserRoles")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ReferenceDataDetail>>> GetAppUserRoles()
        {
            IEnumerable<ReferenceDataDetail> referenceDataDetails = await _ritsDBUnitOfWork.AppUserRoleRepository.FindAsync(
                selector: au => new ReferenceDataDetail()
                {
                    Name = au.Name,
                    Value = au.AppUserRoleId,
                    Active = true
                },
                predicate: r => true == true,
                orderBy: s => s.OrderBy(o => o.Name));

            return Ok(referenceDataDetails);
        }

    }
}
