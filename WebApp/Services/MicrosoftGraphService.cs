using Common.DataTransferObjects.Image;
using Common.DataTransferObjects.MsGraph;
using Newtonsoft.Json;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private readonly HttpClient _httpClient;
        public MicrosoftGraphService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("MicrosoftGraphApiClient");
        }

        public async Task<IEnumerable<MsGraphUserDetail>> FilterUser(string searchString)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                var response = await _httpClient.GetAsync($"v1.0/users?$count=true&$top=10&$filter= " +
                    $"startsWith(givenName,'{searchString}') or " +
                    $"startsWith(surname,'{searchString}') or " +
                    $"startsWith(displayName,'{searchString}') or " +
                    $"startsWith(mail,'{searchString}') or " +
                    $"startsWith(userPrincipalName,'{searchString}')" +
                    $"&$select=id,displayName,mail,employeeId,jobTitle,userPrincipalName,userType");

                if (response.IsSuccessStatusCode)
                {
                    MsGraphUserDetailResult msGraphUserSearchResult = JsonConvert.DeserializeObject<MsGraphUserDetailResult>(await response.Content.ReadAsStringAsync());
                    return msGraphUserSearchResult.Value.Where(v => v.UserType != "Guest");
                }
            }

            return null;
        }

        public async Task<MsGraphUserDetail> GetUserById(string msGraphId)
        {
            if (!string.IsNullOrEmpty(msGraphId))
            {
                var response = await _httpClient.GetAsync($"v1.0/users/{msGraphId}");

                if (response.IsSuccessStatusCode)
                {
                    MsGraphUserDetail msGraphUserDetail = JsonConvert.DeserializeObject<MsGraphUserDetail>(await response.Content.ReadAsStringAsync());
                    return msGraphUserDetail;
                }
            }

            return null;
        }

        public async Task<ImageDetail> GetUserPhotoById(string msGraphId, string size)
        {
            if (!string.IsNullOrEmpty(msGraphId))
            {
                var response = await _httpClient.GetAsync($"v1.0/users/{msGraphId}/photos/{size}/$value");

                if (response.IsSuccessStatusCode)
                {
                    ImageDetail imageDetail = new()
                    {
                        FileContent = await response.Content.ReadAsByteArrayAsync(),
                        ContentType = response.Content.Headers.ContentType.ToString()
                    };

                    return imageDetail;
                }
                else
                {
                    return null;
                }
            }
            return null;
        }
    }
}
