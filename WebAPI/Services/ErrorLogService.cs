using Common.Constants;
using Common.DataTransferObjects.ErrorLog;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPI.Services.Interfaces;

namespace WebAPI.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly string _connectionString;

        public ErrorLogService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ProjectTemplateDB");
        }

        public async Task<ErrorMessage> LogApiError(HttpContext context)
        {
            IExceptionHandlerPathFeature errorHandler = context.Features.Get<IExceptionHandlerPathFeature>();
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            if (errorHandler.Error.GetType() == typeof(SqlException)
                && errorHandler.Error.InnerException != null
                && errorHandler.Error.InnerException.Message == "The network path was not found.")
            {
                return new(context.TraceIdentifier, ErrorMessageTypeConstant.InternalServerException, "DB Connection Error");
            }

            string message = errorHandler.Error.InnerException != null
                    ? $"{errorHandler.Error.Message} | {errorHandler.Error.InnerException.Message}"
                    : errorHandler.Error.Message;

            using (SqlConnection con = new(_connectionString))
            {
                using SqlCommand command = new(StoredProcedureConstant.ErrorLogInsertSpName, con);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_Message, message));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_StackTrace, errorHandler.Error.StackTrace));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_Path, errorHandler.Path));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_StackTraceId, context.TraceIdentifier));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_Source, errorHandler.Error.Source));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_UserIdentity, ClaimService.GetClientId(context.User)));
                command.Parameters.Add(new SqlParameter(StoredProcedureConstant.SpParamName_BuildVerion, string.Format("{0}.{1}.{2}.{3}", version?.Major, version?.Minor, version?.Build, version?.Revision)));

                await con.OpenAsync();
                await command.ExecuteNonQueryAsync();
                await con.CloseAsync();
            }

            if (errorHandler.Error.GetType() == typeof(ArgumentException))
            {
                return new(context.TraceIdentifier, ErrorMessageTypeConstant.ArgumentException, message);
            }

            return new(context.TraceIdentifier, ErrorMessageTypeConstant.InternalServerException, null);
        }
    }
}
