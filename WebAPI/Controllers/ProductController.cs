using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public ProductController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
        }

    }
}
