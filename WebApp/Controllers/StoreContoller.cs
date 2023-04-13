using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.Version;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using WebApp.Extensions;
using WebApp.Models.Error;

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

    }
}
