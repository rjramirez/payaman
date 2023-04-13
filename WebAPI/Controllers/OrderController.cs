using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public OrderController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
        }

    }
}
