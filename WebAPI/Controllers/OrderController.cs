﻿using AutoMapper;
using Common.DataTransferObjects.ReferenceData;
using Common.DataTransferObjects.Order;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using DataAccess.DBContexts.RITSDB.Models;
using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Microsoft.IdentityModel.Tokens;

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
        public async Task<ActionResult<PagedList<OrderSearchResult>>> OrderSearchPagedList([FromQuery] OrderSearchFilter employeeSearchFilter)
        {
            PagedList<OrderSearchResult> OrderSearchResults = await _RITSDBUnitOfWork.OrderRepository.GetPagedListAsync(
                        selector: c => new OrderSearchResult()
                        {
                            Id = c.Id,
                            CashierId = c.CashierId,
                            TotalAmount = c.TotalAmount,
                            CreatedDate = c.CreatedDate,
                            OrderItemList = c.OrderItems.Select(oi => new OrderItemDetail()
                            {
                                Id = oi.Id,
                                ProductId = oi.ProductId,
                                Quantity = oi.Quantity,
                                TotalAmount = oi.TotalAmount,
                                CreatedDate = oi.CreatedDate
                            })
                        },
                        predicate: a =>
                        (
                            string.IsNullOrEmpty(employeeSearchFilter.Keyword) ||
                            (
                                !a.OrderItems.Where(x => x.Product.Name.Contains(employeeSearchFilter.Keyword)).IsNullOrEmpty()
                            )
                        ),
                        pagingParameter: employeeSearchFilter,
                        orderBy: o => o.OrderBy(a => a.Id));

            Response.Headers.Add(PagingConstant.PagingHeaderKey, OrderSearchResults.PagingHeaderValue);
            return Ok(OrderSearchResults);

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
                            ModifiedDate = c.ModifiedDate
                        },
                        predicate: a => a.Active == true,
                        orderBy: o => o.OrderBy(a => a.Id));

            return Ok(Orders);
        }

        [HttpPut]
        [Route("Update")]
        [SwaggerOperation(Summary = "Update Order")]
        public async Task<ActionResult<ClientResponse>> UpdateOrder(OrderDetail OrderDetail)
        {
            var OrderFromDB = await _RITSDBUnitOfWork.OrderRepository.SingleOrDefaultAsync(x => x.Id == OrderDetail.Id);

            var Order = _mapper.Map(OrderDetail, OrderFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(OrderDetail.TransactionBy);

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
        public async Task<ActionResult<ClientResponse>> Remove(OrderDetail OrderDetail)
        {
            var OrderFromDB = await _RITSDBUnitOfWork.OrderRepository.SingleOrDefaultAsync(x => x.Id == OrderDetail.Id);

            OrderFromDB.ModifiedBy = OrderDetail.TransactionBy;

            _RITSDBUnitOfWork.OrderRepository.Remove(OrderFromDB);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(OrderDetail.TransactionBy);

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
        public async Task<ActionResult<ClientResponse>> Add(OrderDetail orderDetail)
        {
            var order = _mapper.Map(orderDetail, new Order());

            order.ModifiedBy = orderDetail.TransactionBy;

            await _RITSDBUnitOfWork.OrderRepository.AddAsync(order);

            var result = await _RITSDBUnitOfWork.SaveChangesAsync(orderDetail.TransactionBy);

            if (result == 0 || result == -1)
                throw new Exception("Adding Order failed");

            ClientResponse clientResponse = new()
            {
                Message = "Order added Successfully",
                IsSuccessful = true,
            };


            return Ok(clientResponse);
        }
    }
}
