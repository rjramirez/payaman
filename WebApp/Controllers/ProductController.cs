﻿using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Employee;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Models.Product;

namespace WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Product/GetAllProducts");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ProductVM> products = JsonConvert.DeserializeObject<IEnumerable<ProductVM>>(await response.Content.ReadAsStringAsync());
                
                //var productList = new JsonResult(new { data = JsonConvert.SerializeObject(products) });
                return Ok(JsonConvert.SerializeObject(products));
            }

            return new JsonResult(new { data = "" });
        }

        //[HttpGet]
        //public async Task<IActionResult> ProductSearch(ProductSearchFilter ProductSearchFilter)
        //{
        //    ProductVM productVM;

        //    HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
        //    HttpResponseMessage response = await client.GetAsync($"api/Product/GetPagedList?{ProductSearchFilter.GetQueryString()}");

        //    if (response.IsSuccessStatusCode)
        //    {
        //        IEnumerable<ProductSearchResult> ProductSearchResults = JsonConvert.DeserializeObject<IEnumerable<ProductSearchResult>>(await response.Content.ReadAsStringAsync());
        //        PagingMetadata pagingMetadata = JsonConvert.DeserializeObject<PagingMetadata>(response.Headers.GetValues(PagingConstant.PagingHeaderKey).FirstOrDefault());
        //        PagedList<ProductSearchResult> result = new(ProductSearchResults.ToList(), pagingMetadata)
        //        {
        //            PageClickEvent = "ControlCenter.ProductChangePage({0})"
        //        };

        //        productVM = new()
        //        {
        //            ProductSearchFilter = ProductSearchFilter,
        //            ProductSearchResults = result
        //        };

        //        return PartialView("_ProductSearchResult", productVM);
        //    }

        //    productVM = new()
        //    {
        //        ProductSearchFilter = ProductSearchFilter,
        //        ProductSearchResults = new PagedList<ProductSearchResult>()
        //    };

        //    return PartialView("_ProductSearchResult", productVM);
        //}

    }
}
