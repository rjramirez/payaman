using Common.DataTransferObjects.Activity;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.ErrorLog;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WebApp.Extensions;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class ActivityCacheService : IActivityCacheService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ClientSetting _clientSetting;

        public ActivityCacheService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ClientSetting clientSetting)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _clientSetting = clientSetting;
        }

        public async Task<IEnumerable<ActivityDetail>> GetActivitiesByCategoryId(int categoryId)
        {
            string cacheKey = $"{nameof(ActivityCacheService)}_{nameof(GetActivitiesByCategoryId)}_{categoryId}";

            if (!_memoryCache.TryGetValue(cacheKey, out IEnumerable<ActivityDetail> activityDetails))
            {
                HttpClient client = _httpClientFactory.CreateClient("RedTagApiClient");
                HttpResponseMessage response = await client.GetAsync($"api/Activity/GetByCategory?CategoryId={categoryId}");

                if (response.IsSuccessStatusCode)
                {
                    activityDetails = JsonConvert.DeserializeObject<IEnumerable<ActivityDetail>>(await response.Content.ReadAsStringAsync());

                    if (activityDetails != null || activityDetails.Any())
                    {
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
                        _memoryCache.Set(cacheKey, activityDetails, cacheEntryOptions);
                    }
                }
                else
                {
                    ErrorMessage errorMessage = await response.GetErrorMessage();
                    throw new ArgumentException(errorMessage.Message);
                }
            }

            return activityDetails;
        }
    }
}
