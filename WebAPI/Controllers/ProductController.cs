using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Employee;
using Common.DataTransferObjects.Product;
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

        [HttpGet]
        [Route("GetAllProducts")]
        [SwaggerOperation(Summary = "Get Product List")]
        public async Task<ActionResult<IEnumerable<ProductDetail>>> GetAll()
        {

            IEnumerable<ProductDetail> products = await _RITSDBUnitOfWork.ProductRepository.FindAsync(
                        selector: c => new ProductDetail()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Price = c.Price,
                            Description = c.Description,
                            CreatedDate = c.CreatedDate
                        },
                        predicate: a => !String.IsNullOrEmpty(a.Name),
                        orderBy: o => o.OrderBy(a => a.Name));

            return Ok(products);
        }

    }
}
