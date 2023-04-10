using Common.DataTransferObjects.ErrorLog;

namespace WebAPI.Services.Interfaces
{
    public interface IErrorLogService
    {
        Task<ErrorMessage> LogApiError(HttpContext context);
    }
}
