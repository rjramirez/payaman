using Common.Constants;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Common.DataTransferObjects.AppUserDetails;
using WebApp.Models.AppUser;

namespace WebApp.Controllers
{
    [Authorize]
    public class AppUserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AppUserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAppUsers()
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/AppUser/GetAllAppUsers");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<AppUserDetail> appUsers = JsonConvert.DeserializeObject<IEnumerable<AppUserDetail>>(await response.Content.ReadAsStringAsync());

                List<AppUserVM> appUserVM = appUsers.Select(au => new AppUserVM()
                {
                    AppUserId = au.AppUserId,
                }).ToList();

                return Ok(JsonConvert.SerializeObject(appUsers));
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AppUserDetail appUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            appUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PutAsync($"api/AppUser/Update", appUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] AppUserDetail AppUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            AppUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/AppUser/Remove", AppUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AppUserDetail AppUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            AppUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/AppUser/Add", AppUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

    }
}
