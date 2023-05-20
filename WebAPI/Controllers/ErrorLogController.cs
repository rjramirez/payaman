using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "SystemLog")]
    public class ErrorLogController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public ErrorLogController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
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

            await _RITSDBUnitOfWork.ErrorLogRepository.AddAsync(errorLog);
            await _RITSDBUnitOfWork.SaveChangesAsync(saveErrorLog.UserIdentity);

            return Ok();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Error details by ID")]
        public async Task<ActionResult<ErrorLogDetail>> Get(int id)
        {
            ErrorLogDetail errorLogDetail = await _RITSDBUnitOfWork.ErrorLogRepository.
                FirstOrDefaultAsync(selector: e => new ErrorLogDetail()
                {
                    BuildVersion = e.BuildVersion,
                    DateCreated = e.DateCreated,
                    ErrorLogId = e.ErrorLogId,
                    Message = e.Message,
                    Path = e.Path,
                    Source = e.Source,
                    StackTrace = e.StackTrace,
                    StackTraceId = e.StackTraceId,
                    UserIdentity = e.UserIdentity
                },
                predicate: e => e.ErrorLogId == id);

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
            PagedList<ErrorLogDetail> errorLogDetails = await _RITSDBUnitOfWork.ErrorLogRepository.GetPagedListAsync(
                selector: e => new ErrorLogDetail()
                {
                    ErrorLogId = e.ErrorLogId,
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
