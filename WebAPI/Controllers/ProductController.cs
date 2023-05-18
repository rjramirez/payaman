using AutoMapper;
using Common.DataTransferObjects.Product;
using Common.DataTransferObjects.ReferenceData;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        private readonly IMapper _mapper;
        public ProductController(IRITSDBUnitOfWork RITSDBUnitOfWork, IMapper mapper)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
            _mapper = mapper;
        }


        //[HttpGet]
        //[Route("GetPagedList")]
        //[SwaggerOperation(Summary = "Get Product Paged List")]
        //public async Task<ActionResult<PagedList<ProductSearchResult>>> GetPagedList([FromQuery] ProductSearchFilter employeeSearchFilter)
        //{

        //    PagedList<ProductSearchResult> productSearchResults = await _RITSDBUnitOfWork.ProductRepository.GetPagedListAsync(
        //                selector: c => new ProductSearchResult()
        //                {
        //                    Name = c.Name,
        //                    Price = c.Price
        //                },
        //                predicate: a =>
        //                (
        //                    string.IsNullOrEmpty(employeeSearchFilter.Keyword) ||
        //                    (
        //                        a.Name.Contains(employeeSearchFilter.Keyword) ||
        //                        a.Price.ToString().Contains(employeeSearchFilter.Keyword)
        //                    )
        //                ),
        //                pagingParameter: employeeSearchFilter,
        //                orderBy: o => o.OrderBy(a => a.Name));

        //    Response.Headers.Add(PagingConstant.PagingHeaderKey, productSearchResults.PagingHeaderValue);
        //    return Ok(productSearchResults);

        //}

        [HttpGet]
        [Route("GetAllProductsByStoreId/{storeId}")]
        [SwaggerOperation(Summary = "Get Product List")]
        public async Task<ActionResult<IEnumerable<ProductDetail>>> GetAllProducts([FromRoute] string storeId)
        {
            int storeIdSelected = Convert.ToInt32(storeId);

            IEnumerable<ProductDetail> products = await _RITSDBUnitOfWork.ProductRepository.FindAsync(
                        selector: c => new ProductDetail()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Price = c.Price.ToString(),
                            Description = c.Description,
                            StoreId = c.StoreId,
                            CreatedDate = c.CreatedDate
                        },
                        predicate: a => a.Active == true && (storeIdSelected > 0 && storeIdSelected == a.StoreId),
                        orderBy: o => o.OrderBy(a => a.Name));

            return Ok(products);
        }

        [HttpPut]
        [Route("Update")]
        [SwaggerOperation(Summary = "Update Product")]
        public async Task<ActionResult<ClientResponse>> UpdateProduct(ProductDetail productDetail)
        {
            var productFromDB = await _RITSDBUnitOfWork.ProductRepository.SingleOrDefaultAsync(x => x.Id == productDetail.Id);

            var product = _mapper.Map(productDetail, productFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(productDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Updating product failed");

            ClientResponse clientResponse = new()
            {
                Message = "Product updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Remove")]
        [SwaggerOperation(Summary = "Remove Product")]
        public async Task<ActionResult<ClientResponse>> Remove(ProductDetail productDetail)
        {
            var productFromDB = await _RITSDBUnitOfWork.ProductRepository.SingleOrDefaultAsync(x => x.Id == productDetail.Id);

            productFromDB.ModifiedBy = productDetail.TransactionBy;
            productFromDB.ModifiedDate = Convert.ToDateTime(DateTime.UtcNow.ToString("MMMM dd, yyyy H:m"));
            productFromDB.Active = false;

            //_RITSDBUnitOfWork.ProductRepository.Remove(productFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(productDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Removing product failed");

            ClientResponse clientResponse = new()
            {
                Message = "Product updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Add")]
        [SwaggerOperation(Summary = "Add Product")]
        public async Task<ActionResult<ClientResponse>> Add(ProductDetail productDetail)
        {
            var product = _mapper.Map(productDetail, new Product());

            product.ModifiedBy = productDetail.TransactionBy;
            product.Active = true;



            await _RITSDBUnitOfWork.ProductRepository.AddAsync(product);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(productDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Adding product failed");

            ClientResponse clientResponse = new()
            {
                Message = "Product added Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

    }
}
