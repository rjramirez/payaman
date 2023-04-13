using Common.Constants;
using Common.DataTransferObjects.AuditTrail;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "SystemLog")]
    [ApiVersion("1.0")]
    public class AuditTrailController : ControllerBase
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;
        public AuditTrailController(IRITSDBUnitOfWork RITSDBUnitOfWork)
        {
            _RITSDBUnitOfWork = RITSDBUnitOfWork;
        }

        [HttpGet]
        [Route("AuditTrailSearch")]
        [SwaggerOperation(Summary = "Search Audit Trail with Paging")]
        public async Task<ActionResult<PagedList<AuditTrailLog>>> Search([FromQuery] CommonSearchFilter commonSearchFilter)
        {
            PagedList<AuditTrailLog> auditTrailLogs = await _RITSDBUnitOfWork.AuditTrailRepository.GetPagedListAsync(
                selector: a => new AuditTrailLog()
                {
                    AuditTrailId = a.AuditTrailId,
                    TransactionBy = a.TransactionBy,
                    TransactionDate = a.TransactionDate,
                    TotalDataChanges = a.AuditTrailDetails.Count,
                    TotalAffectedTables = a.AuditTrailDetails.Select(ad => ad.TableName).Distinct().Count()
                },
                predicate: a => (commonSearchFilter.StartDate == null || a.TransactionDate >= commonSearchFilter.StartDate) &&
                                 (commonSearchFilter.EndDate == null || a.TransactionDate <= commonSearchFilter.EndDate) &&
                                 (string.IsNullOrEmpty(commonSearchFilter.SearchKeyword) || a.TransactionBy.Contains(commonSearchFilter.SearchKeyword)),
                pagingParameter: commonSearchFilter,
                orderBy: o => o.OrderByDescending(a => a.TransactionDate));

            Response.Headers.Add(PagingConstant.PagingHeaderKey, auditTrailLogs.PagingHeaderValue);

            return Ok(auditTrailLogs);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Audit trail details by ID")]
        public async Task<ActionResult<AuditTrailLogDetail>> Get(long id)
        {

            AuditTrailLogDetail auditTrailLogDetail = await _RITSDBUnitOfWork.AuditTrailRepository.SingleOrDefaultAsync(
                selector: a => new AuditTrailLogDetail()
                {
                    AuditTrailId = a.AuditTrailId,
                    TransactionBy = a.TransactionBy,
                    TransactionDate = a.TransactionDate,
                    TotalDataChanges = a.AuditTrailDetails.Count,
                    TotalAffectedTables = a.AuditTrailDetails.Select(ad => ad.TableName).Distinct().Count(),
                    AuditTrailChanges = a.AuditTrailDetails.Select(ad => new AuditTrailChange()
                    {
                        PrimaryKey = ad.EntityId,
                        TableName = ad.TableName,
                        ColumnName = ad.EntityField,
                        OldValue = ad.OldValue,
                        NewValue = ad.NewValue
                    })
                },
                predicate: a => a.AuditTrailId == id);

            if (auditTrailLogDetail == null)
            {
                return NotFound(new ErrorMessage(ErrorMessageTypeConstant.NotFound, $"AuditTrail ID not exist: {id}"));
            }
            else
            {
                return Ok(auditTrailLogDetail);
            }
        }
    }
}
