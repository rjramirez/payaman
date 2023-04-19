using Common.DataTransferObjects.Activity;

namespace WebApp.Services.Interfaces
{
    public interface IActivityCacheService
    {
        Task<IEnumerable<ActivityDetail>> GetActivitiesByCategoryId(int categoryId);
    }
}
