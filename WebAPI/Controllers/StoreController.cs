using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public StoreController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
        }

    }
}
