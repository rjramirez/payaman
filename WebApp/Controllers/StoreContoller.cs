using Common.Constants;
using Common.DataTransferObjects.Store;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApp.Models.Store;

namespace WebApp.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StoreController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllStores()
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Store/GetAllStores");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<StoreDetail> stores = JsonConvert.DeserializeObject<IEnumerable<StoreDetail>>(await response.Content.ReadAsStringAsync());
                return Ok(stores);
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StoreDetail storeDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            storeDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PutAsync($"api/Store/Update", storeDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] StoreDetail storeDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            storeDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Store/Remove", storeDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] StoreDetail storeDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            storeDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Store/Add", storeDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

    }
}
