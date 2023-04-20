using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Employee;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "SystemData")]
    public class ProductController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public ProductController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
        }


        [HttpGet]
        [Route("GetPagedList")]
        [SwaggerOperation(Summary = "Get Product List")]
        public async Task<ActionResult<PagedList<ProductSearchResult>>> GetPagedList([FromQuery] ProductSearchFilter employeeSearchFilter)
        {

            PagedList<ProductSearchResult> productSearchResults = await _RITSDBUnitOfWork.ProductRepository.GetPagedListAsync(
                        selector: c => new ProductSearchResult()
                        {
                            Name = c.Name,
                            Price = c.Price
                        },
                        predicate: a =>
                        (
                            string.IsNullOrEmpty(employeeSearchFilter.Keyword) ||
                            (
                                a.Name.Contains(employeeSearchFilter.Keyword) ||
                                a.Price.ToString().Contains(employeeSearchFilter.Keyword)
                            )
                        ),
                        pagingParameter: employeeSearchFilter,
                        orderBy: o => o.OrderBy(a => a.Name));

            Response.Headers.Add(PagingConstant.PagingHeaderKey, productSearchResults.PagingHeaderValue);
            return Ok(productSearchResults);

        }

    }
}
