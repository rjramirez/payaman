using Common.Constants;
using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Principal;
using WebApp.Extensions;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ClientSetting _clientSetting;

        public CommonService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ClientSetting clientSetting)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _clientSetting = clientSetting;
        }

        public string ConvertBoleanToYesOrNo(bool bolean)
        {
            if (bolean)
            {
                return "Yes";
            }
            else
            {
                return "No";
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetReferenceDataSelectList(string apiEndpoint, string selectedValue, bool enableCache)
        {
            IEnumerable<ReferenceDataDetail> referenceDataDetails = await GetReferenceDataFromApi(apiEndpoint, enableCache);

            IEnumerable<SelectListItem> selectListItems = referenceDataDetails.Select(r => new SelectListItem()
            {
                Value = r.Value.ToString(),
                Text = r.Name,
                Selected = selectedValue != null && selectedValue == r.Value.ToString()
            });

            return selectListItems;
        }

        public async Task<IEnumerable<SelectListItem>> GetReferenceDataSelectList(string apiEndpoint, IEnumerable<string> selectedValues, bool enableCache)
        {
            IEnumerable<ReferenceDataDetail> referenceDataDetails = await GetReferenceDataFromApi(apiEndpoint, enableCache);

            IEnumerable<SelectListItem> selectListItems = referenceDataDetails.Select(r => new SelectListItem()
            {
                Value = r.Value.ToString(),
                Text = r.Name,
                Selected = selectedValues != null && selectedValues.Contains(r.Value.ToString())
            });

            return selectListItems;
        }

        public async Task<IEnumerable<ReferenceDataDetail>> GetReferenceDataFromApi(string apiEndpoint, bool enableCache)
        {
            if (!_memoryCache.TryGetValue(apiEndpoint, out IEnumerable<ReferenceDataDetail> referenceDataDetails))
            {
                var client = _httpClientFactory.CreateClient("RITSApiClient");
                var response = await client.GetAsync(apiEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    referenceDataDetails = JsonConvert.DeserializeObject<IEnumerable<ReferenceDataDetail>>(await response.Content.ReadAsStringAsync());

                    if (enableCache && (referenceDataDetails != null || referenceDataDetails.Any()))
                    {
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
                        _memoryCache.Set(apiEndpoint, referenceDataDetails, cacheEntryOptions);
                    }
                }
                else
                {
                    ErrorMessage errorMessage = await response.GetErrorMessage();
                    throw new ArgumentException(errorMessage.Message);
                }
            }

            return referenceDataDetails;
        }

        public async Task<ReferenceDataDetail> GetUserRole(IPrincipal principal)
        {
            HttpClient ritsHttpClient = _httpClientFactory.CreateClient("RITSApiClient");
            string userRolesCacheName = string.Format(RoleConstant.UserRoleCacheName, principal.Identity.Name);
            if (!_memoryCache.TryGetValue(userRolesCacheName, out ReferenceDataDetail userRole))
            {
                var response = await ritsHttpClient.GetAsync($"api/User/UserRole/{principal.Identity.Name}");
                if (response.IsSuccessStatusCode)
                {
                    userRole = JsonConvert.DeserializeObject<ReferenceDataDetail>(await response.Content.ReadAsStringAsync());
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
                    _memoryCache.Set(userRolesCacheName, userRole, cacheEntryOptions);
                }
                else
                {
                    return new ReferenceDataDetail();
                }
            }

            return userRole;
        }

    }
}
