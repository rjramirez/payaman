using Common.Constants;
using Common.DataTransferObjects.Order;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApp.Models.Order;

namespace WebApp.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Order/GetAllOrders");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<OrderVM> Orders = JsonConvert.DeserializeObject<IEnumerable<OrderVM>>(await response.Content.ReadAsStringAsync());
                return Ok(JsonConvert.SerializeObject(Orders));
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] OrderDetail OrderDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            OrderDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PutAsync($"api/Order/Update", OrderDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] OrderDetail OrderDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            OrderDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Order/Remove", OrderDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] OrderDetail OrderDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            OrderDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.ClientId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Order/Add", OrderDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

    }
}
