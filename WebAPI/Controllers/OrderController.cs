using AutoMapper;
using Common.DataTransferObjects.ReferenceData;
using Common.DataTransferObjects.Order;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using DataAccess.DBContexts.RITSDB.Models;
using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using DataAccess.Repositories.RITSDB;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        private readonly IMapper _mapper;
        public OrderController(IRITSDBUnitOfWork RITSDBUnitOfWork, IMapper mapper)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
            _mapper = mapper;
        }


        [HttpGet]
        [Route("Search")]
        [SwaggerOperation(Summary = "Search and Get Order Paged List")]
        public async Task<ActionResult<PagedList<OrderSearchResult>>> OrderSearchPagedList([FromQuery] OrderSearchFilter orderSearchFilter)
        {
            if (orderSearchFilter.StoreId == 0)
            {
                int firstStoreId = Convert.ToInt32(await _RITSDBUnitOfWork.StoreRepository.FirstOrDefaultAsync(
                        selector: x => x.Id,
                        predicate: x => x.Active == true));

                orderSearchFilter.StoreId = firstStoreId;
            }

            PagedList<OrderSearchResult> orderSearchResults = await _RITSDBUnitOfWork.ProductRepository
                .GetPagedListAsync(
                        selector: o => new OrderSearchResult()
                        {
                            Id = o.Id,
                            ProductId = o.Id,
                            ProductName = o.Name,
                            ProductPrice = o.Price,
                            Quantity = 0,
                            CreatedDate = o.CreatedDate
                        },
                        predicate: o =>
                        (
                            o.Active == true &&
                            (string.IsNullOrEmpty(orderSearchFilter.Keyword) || o.Name.Contains(orderSearchFilter.Keyword)) &&
                            (orderSearchFilter.StoreId == 0 || o.StoreId == orderSearchFilter.StoreId)
                        ),
                        pagingParameter: orderSearchFilter,
                        orderBy: o => o.OrderBy(a => a.CreatedDate));

            Response.Headers.Add(PagingConstant.PagingHeaderKey, orderSearchResults.PagingHeaderValue);

            //Add quantity to each product order as of today
            foreach (var item in orderSearchResults)
            {
                item.Quantity = GetQuantityOrderItems(item.ProductId).Result;
            }

            return Ok(orderSearchResults);
        }

        private async Task<int> GetQuantityOrderItems(int productId) 
        {
            int count = await _RITSDBUnitOfWork.OrderItemRepository.CountAsync(predicate: x => x.ProductId == productId &&
            //Get orders today only
            DateTime.Compare(x.CreatedDate, DateTime.Now) == 0);
            return count;
        }

        [HttpGet]
        [Route("GetAllOrders")]
        [SwaggerOperation(Summary = "Get Order List")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetAll()
        {
            IEnumerable<OrderDetail> Orders = await _RITSDBUnitOfWork.OrderRepository.FindAsync(
                        selector: c => new OrderDetail()
                        {
                            Id = c.Id,
                            TotalAmount = c.TotalAmount,
                            CreatedDate = c.CreatedDate,
                            ModifiedDate = c.ModifiedDate.Value
                        },
                        predicate: a => a.Active == true,
                        orderBy: o => o.OrderBy(a => a.Id));

            return Ok(Orders);
        }

        [HttpPut]
        [Route("Update")]
        [SwaggerOperation(Summary = "Update Order")]
        public async Task<ActionResult<ClientResponse>> UpdateOrder(OrderDetail orderDetail)
        {
            var orderFromDB = await _RITSDBUnitOfWork.OrderRepository.SingleOrDefaultAsync(x => x.Id == orderDetail.Id);

            var order = _mapper.Map(orderDetail, orderFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(orderDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Updating Order failed");

            ClientResponse clientResponse = new()
            {
                Message = "Order updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Remove")]
        [SwaggerOperation(Summary = "Remove Order")]
        public async Task<ActionResult<ClientResponse>> Remove(OrderDetail orderDetail)
        {
            var orderFromDB = await _RITSDBUnitOfWork.OrderRepository.SingleOrDefaultAsync(x => x.Id == orderDetail.Id);
            
            var today = DateTime.UtcNow;
            orderFromDB.ModifiedBy = orderDetail.TransactionBy;
            orderFromDB.ModifiedDate = Convert.ToDateTime(today.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            orderFromDB.Active = false;

            _RITSDBUnitOfWork.OrderRepository.Remove(orderFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(orderDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Removing Order failed");

            ClientResponse clientResponse = new()
            {
                Message = "Order updated Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }

        [HttpPost]
        [Route("Add")]
        [SwaggerOperation(Summary = "Add Order")]
        public async Task<ActionResult<int>> Add(OrderDetail orderDetail)
        {
            var order = _mapper.Map(orderDetail, new Order());

            var today = DateTime.UtcNow;

            order.CashierId = Convert.ToInt16(orderDetail.TransactionBy);
            order.CreatedBy = orderDetail.TransactionBy;
            order.CreatedDate = Convert.ToDateTime(today.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            order.Active = true;

            await _RITSDBUnitOfWork.OrderRepository.AddAsync(order);
            await _RITSDBUnitOfWork.SaveChangesAsync(orderDetail.TransactionBy);

            foreach (var orderItem in orderDetail.OrderItemList)
            {
                var orderItemForInsert = _mapper.Map(orderItem, new OrderItem());
                orderItemForInsert.OrderId = order.Id;
                orderItemForInsert.CreatedBy = orderDetail.TransactionBy;
                orderItemForInsert.CreatedDate = Convert.ToDateTime(today.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                orderItemForInsert.Active = true;

                await _RITSDBUnitOfWork.OrderItemRepository.AddAsync(orderItemForInsert);
            }

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(orderDetail.TransactionBy);


            if (result == 0 || result == -1)
                throw new Exception("Adding Order failed");


            return Ok(order.Id);
        }

    }
}
