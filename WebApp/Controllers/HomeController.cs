using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using Common.DataTransferObjects.ErrorLog;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.Extensions;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IMapper _mapper;
        public HomeController(IHttpClientFactory httpClientFactory, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AuthenticationCallback([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var authRequest = _mapper.Map<AuthenticateRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/authenticate", authRequest.GetStringContent());

            if (response.IsSuccessStatusCode)
            {
                ErrorLogDetail errorLogDetail = JsonConvert.DeserializeObject<ErrorLogDetail>(await response.Content.ReadAsStringAsync());
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
            }
            
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}