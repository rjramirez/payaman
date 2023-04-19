using Common.DataTransferObjects.AppSettings;
using Common.DataTransferObjects.Employee;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WebApp.Extensions;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class GlobalMappingService : IGlobalMappingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly ClientSetting _clientSetting;

        public GlobalMappingService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, ClientSetting clientSetting)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _clientSetting = clientSetting;
        }

        public async Task<IEnumerable<ReferenceDataDetail>> FilterReference(string endpoint, bool enableCache)
        {

            if (!_memoryCache.TryGetValue(endpoint, out IEnumerable<ReferenceDataDetail> referenceDataDetails))
            {

                var client = _httpClientFactory.CreateClient("RedTagApiClient");
                var response = await client.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    referenceDataDetails = JsonConvert.DeserializeObject<IEnumerable<ReferenceDataDetail>>(await response.Content.ReadAsStringAsync());


                    if (enableCache && (referenceDataDetails != null || referenceDataDetails.Any()))
                    {
                        MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
                        _memoryCache.Set(endpoint, referenceDataDetails, cacheEntryOptions);
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

        public async Task<IEnumerable<EmployeeDetail>> FilterEmployee(string searchString, bool enableCache)
        {
            string endpoint = $"api/Employee/EmployeeSearch?Keyword={searchString}";

            if (!_memoryCache.TryGetValue(endpoint, out IEnumerable<EmployeeDetail> employeeDetailSearchResults))
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    HttpClient client = _httpClientFactory.CreateClient("RedTagApiClient");
                    HttpResponseMessage response = await client.GetAsync(endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        employeeDetailSearchResults = JsonConvert.DeserializeObject<IEnumerable<EmployeeDetail>>(await response.Content.ReadAsStringAsync());

                        if (enableCache && (employeeDetailSearchResults != null || employeeDetailSearchResults.Any()))
                        {
                            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
                            _memoryCache.Set(endpoint, employeeDetailSearchResults, cacheEntryOptions);
                        }

                        return employeeDetailSearchResults;
                    }
                    else
                    {
                        ErrorMessage errorMessage = await response.GetErrorMessage();
                        throw new ArgumentException(errorMessage.Message);
                    }
                }

                return null;
            }

            return employeeDetailSearchResults;
        }
    }
}
