using AutoMapper;
using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Employee;
using Common.DataTransferObjects.Product;
using Common.DataTransferObjects.ReferenceData;
using DataAccess.DBContexts.RITSDB.Models;
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
        private readonly IMapper _mapper;
        public ProductController(IRITSDBUnitOfWork RITSDBUnitOfWork, IMapper mapper)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
            _mapper = mapper;
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

        [HttpPut]
        [Route("UpdateProduct")]
        [SwaggerOperation(Summary = "Update Product")]
        public async Task<ActionResult<ClientResponse>> UpdateProduct(ProductDetail productDetail)
        {
            var productFromDB = await _RITSDBUnitOfWork.ProductRepository.SingleOrDefaultAsync(x => x.Id == productDetail.Id);

            var product = _mapper.Map(productDetail, productFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(productDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Updating product went unsuccessful");

            ClientResponse clientResponse = new()
            {
                Message = "Product updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

    }
}
