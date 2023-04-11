using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;
using DataAccess.DBContexts.PayamanDB.Models;
using DataAccess.UnitOfWorks.PayamanDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SystemLog")]
    public class ErrorLogController : ControllerBase
    {
        private readonly IPayamanDBUnitOfWork _PayamanDBUnitOfWork;
        public ErrorLogController(IPayamanDBUnitOfWork PayamanDBUnitOfWork)
        {
            _PayamanDBUnitOfWork = PayamanDBUnitOfWork;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Log appilcation error")]
        public async Task<IActionResult> Create(SaveErrorLog saveErrorLog)
        {
            ErrorLog errorLog = new()
            {
                BuildVersion = saveErrorLog.BuildVersion,
                DateCreated = DateTime.Now,
                Message = saveErrorLog.Message,
                Path = saveErrorLog.Path,
                Source = saveErrorLog.Source,
                StackTrace = saveErrorLog.StackTrace,
                StackTraceId = saveErrorLog.StackTraceId,
                UserIdentity = saveErrorLog.UserIdentity
            };

            await _PayamanDBUnitOfWork.ErrorLogRepository.AddAsync(errorLog);
            await _PayamanDBUnitOfWork.SaveChangesAsync(saveErrorLog.UserIdentity);

            return Ok();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Error details by ID")]
        public async Task<ActionResult<ErrorLogDetail>> Get(int id)
        {
            ErrorLogDetail errorLogDetail = await _PayamanDBUnitOfWork.ErrorLogRepository.
                FirstOrDefaultAsync(selector: e => new ErrorLogDetail()
                {
                    BuildVersion = e.BuildVersion,
                    DateCreated = e.DateCreated,
                    ErrorId = e.ErrorId,
                    Message = e.Message,
                    Path = e.Path,
                    Source = e.Source,
                    StackTrace = e.StackTrace,
                    StackTraceId = e.StackTraceId,
                    UserIdentity = e.UserIdentity
                },
                predicate: e => e.ErrorId == id);

            if (errorLogDetail == null)
            {
                return NotFound(new ErrorMessage(ErrorMessageTypeConstant.NotFound, $"Error ID not exist: {id}"));
            }
            else
            {
                return Ok(errorLogDetail);
            }
        }

        [HttpGet]
        [Route("ErrorLogListPagedSearch")]
        [SwaggerOperation(Summary = "Search Error Log with Paging")]
        public async Task<ActionResult<PagedList<ErrorLogDetail>>> Search([FromQuery] CommonSearchFilter commonSearchFilter)
        {
            PagedList<ErrorLogDetail> errorLogDetails = await _PayamanDBUnitOfWork.ErrorLogRepository.GetPagedListAsync(
                selector: e => new ErrorLogDetail()
                {
                    ErrorId = e.ErrorId,
                    BuildVersion = e.BuildVersion,
                    DateCreated = e.DateCreated,
                    Message = e.Message,
                    Path = e.Path,
                    Source = e.Source,
                    StackTrace = e.StackTrace,
                    StackTraceId = e.StackTraceId,
                    UserIdentity = e.UserIdentity
                },
                predicate: e => ((commonSearchFilter.StartDate == null || e.DateCreated >= commonSearchFilter.StartDate) &&
                                 (commonSearchFilter.EndDate == null || e.DateCreated <= commonSearchFilter.EndDate)) &&
                                 (string.IsNullOrEmpty(commonSearchFilter.SearchKeyword) || (e.BuildVersion.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.Message.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.Path.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.Source.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.StackTrace.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.StackTraceId.Contains(commonSearchFilter.SearchKeyword) ||
                                                                          e.UserIdentity.Contains(commonSearchFilter.SearchKeyword))),
                pagingParameter: commonSearchFilter,
                orderBy: o => o.OrderByDescending(e => e.DateCreated));

            Response.Headers.Add(PagingConstant.PagingHeaderKey, errorLogDetails.PagingHeaderValue);

            return Ok(errorLogDetails);
        }
    }
}
