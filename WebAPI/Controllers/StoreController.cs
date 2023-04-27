using Common.DataTransferObjects.Store;
using Common.DataTransferObjects.ReferenceData;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using AutoMapper;
using System;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        private readonly IMapper _mapper;
        public StoreController(IRITSDBUnitOfWork RITSDBUnitOfWork, IMapper mapper)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
            _mapper = mapper;
        }


        //[HttpGet]
        //[Route("GetPagedList")]
        //[SwaggerOperation(Summary = "Get Store Paged List")]
        //public async Task<ActionResult<PagedList<StoreSearchResult>>> GetPagedList([FromQuery] StoreSearchFilter employeeSearchFilter)
        //{

        //    PagedList<StoreSearchResult> StoreSearchResults = await _RITSDBUnitOfWork.StoreRepository.GetPagedListAsync(
        //                selector: c => new StoreSearchResult()
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

        //    Response.Headers.Add(PagingConstant.PagingHeaderKey, StoreSearchResults.PagingHeaderValue);
        //    return Ok(StoreSearchResults);

        //}

        [HttpGet]
        [Route("GetAllStores")]
        [SwaggerOperation(Summary = "Get Store List")]
        public async Task<ActionResult<IEnumerable<StoreDetail>>> GetAll()
        {

            IEnumerable<StoreDetail> Stores = await _RITSDBUnitOfWork.StoreRepository.FindAsync(
                        selector: c => new StoreDetail()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Image = c.Image,
                            CreatedDate = c.CreatedDate.Value
                        },
                        predicate: a => !String.IsNullOrEmpty(a.Name),
                        orderBy: o => o.OrderBy(a => a.Name));

            return Ok(Stores);
        }

        [HttpPut]
        [Route("Update")]
        [SwaggerOperation(Summary = "Update Store")]
        public async Task<ActionResult<ClientResponse>> UpdateStore(StoreDetail StoreDetail)
        {
            var StoreFromDB = await _RITSDBUnitOfWork.StoreRepository.SingleOrDefaultAsync(x => x.Id == StoreDetail.Id);

            var Store = _mapper.Map(StoreDetail, StoreFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(StoreDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Updating Store failed");

            ClientResponse clientResponse = new()
            {
                Message = "Store updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Remove")]
        [SwaggerOperation(Summary = "Remove Store")]
        public async Task<ActionResult<ClientResponse>> Remove(StoreDetail StoreDetail)
        {
            var StoreFromDB = await _RITSDBUnitOfWork.StoreRepository.SingleOrDefaultAsync(x => x.Id == StoreDetail.Id);

            StoreFromDB.ModifiedBy = StoreDetail.TransactionBy;

            _RITSDBUnitOfWork.StoreRepository.Remove(StoreFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(StoreDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Removing Store failed");

            ClientResponse clientResponse = new()
            {
                Message = "Store updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Add")]
        [SwaggerOperation(Summary = "Add Store")]
        public async Task<ActionResult<ClientResponse>> Add(StoreDetail StoreDetail)
        {
            var store = _mapper.Map(StoreDetail, new Store());

            var today = DateTime.UtcNow;
            //store.CreatedDate = today.ToString("yyyy-MM-dd HH:mm:ss.fff");

            await _RITSDBUnitOfWork.StoreRepository.AddAsync(store);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(StoreDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Adding Store failed");

            ClientResponse clientResponse = new()
            {
                Message = "Store added Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }
    }
}
