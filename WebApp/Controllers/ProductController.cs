using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Order;
using Common.DataTransferObjects.Product;
using Common.DataTransferObjects.ReferenceData;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApp.Extensions;
using WebApp.Models.Cart;
using WebApp.Models.Order;
using WebApp.Models.Product;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public ProductController(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }


        [HttpGet]
        [Route("Product/GetAllProducts/{storeId}")]
        public async Task<IActionResult> GetAllProducts([FromRoute] string storeId)
        {
            string storeSelectedCacheName = string.Format(CacheConstant.StoreSelectedCacheName, User.Identity.Name);

            if (_memoryCache.TryGetValue(storeSelectedCacheName, out IEnumerable<ReferenceDataDetail> storeSelectedCached) || (storeId != "undefined" || Convert.ToInt32(storeId) > 0))
            {
                string storeIdSelected = storeSelectedCached.SingleOrDefault(r => r.Name == CacheConstant.StoreIdCacheName).Value.ToString();
                HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

                //Overwrite if storeId is defined
                if (storeId != "undefined" && Convert.ToInt32(storeId) > 0)
                    storeIdSelected = storeId;

                HttpResponseMessage response = await client.GetAsync($"api/Product/GetAllProductsByStoreId/{storeIdSelected}");

                if (response.IsSuccessStatusCode)
                {
                    IEnumerable<ProductVM> products = JsonConvert.DeserializeObject<IEnumerable<ProductVM>>(await response.Content.ReadAsStringAsync());

                    //var productList = new JsonResult(new { data = JsonConvert.SerializeObject(products) });
                    return Ok(JsonConvert.SerializeObject(products));
                }
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProductDetail productDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            productDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PutAsync($"api/Product/Update", productDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] ProductDetail productDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            productDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Product/Remove", productDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ProductDetail productDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            productDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.PostAsync($"api/Product/Add", productDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

    }
}
