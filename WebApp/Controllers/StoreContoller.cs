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
                IEnumerable<StoreVM> Stores = JsonConvert.DeserializeObject<IEnumerable<StoreVM>>(await response.Content.ReadAsStringAsync());
                return Ok(JsonConvert.SerializeObject(Stores));
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] StoreDetail StoreDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            StoreDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PutAsync($"api/Store/Update", StoreDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] StoreDetail StoreDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            StoreDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Store/Remove", StoreDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] StoreDetail StoreDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            StoreDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Store/Add", StoreDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

    }
}
