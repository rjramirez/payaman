using Common.Constants;
using Common.DataTransferObjects.ErrorLog;
using DataAccess.DBContexts.RITSDB.Models;
using DataAccess.UnitOfWorks.RITSDB;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly IRITSDBUnitOfWork _RITSDBUnitOfWork;

        public ErrorLogService(IRITSDBUnitOfWork ritsDBUnitOfWork)
        {
            _RITSDBUnitOfWork = ritsDBUnitOfWork;
        }

        public async Task<ErrorMessage> LogApiError(HttpContext context)
        {
            IExceptionHandlerPathFeature errorHandler = context.Features.Get<IExceptionHandlerPathFeature>();
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; if (errorHandler.Error.GetType() == typeof(SqlException) && errorHandler.Error.InnerException != null && errorHandler.Error.InnerException.Message == "The network path was not found.")
            {
                return new(context.TraceIdentifier, ErrorMessageTypeConstant.InternalServerException, "DB Connection Error");
            }
            if ((errorHandler.Error.InnerException != null && errorHandler.Error.InnerException.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint")) ||
            (errorHandler.Error.Message != null && errorHandler.Error.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint")))
            {
                return new(context.TraceIdentifier, ErrorMessageTypeConstant.InternalServerException, "Cannot delete entity because it is being referenced or used by other entities.");
            }
            string message = errorHandler.Error.InnerException != null
            ? $"{errorHandler.Error.Message} | {errorHandler.Error.InnerException.Message}"
            : errorHandler.Error.Message; ErrorLog errorLog = new ErrorLog()
            {
                Message = message,
                StackTrace = errorHandler.Error.StackTrace,
                Path = errorHandler.Path,
                StackTraceId = context.TraceIdentifier,
                Source = errorHandler.Error.Source,
                UserIdentity = ClaimService.GetClientId(context.User),
                BuildVersion = string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision),
                DateCreated = DateTime.Now
            };

            await _RITSDBUnitOfWork.ErrorLogRepository.AddAsync(errorLog);
            await _RITSDBUnitOfWork.SaveChangesAsync(ClaimService.GetClientId(context.User)); if (errorHandler.Error.GetType() == typeof(ArgumentException))
            {
                return new(context.TraceIdentifier, ErrorMessageTypeConstant.ArgumentException, message);
            }
            return new(context.TraceIdentifier, ErrorMessageTypeConstant.InternalServerException, null);
        }
    }
}
