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
            var response = await client.PostAsync($"api/User/Authenticate", authRequest.GetStringContent());

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var responseFail = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
                if (responseFail == null)
                {
                    responseFail = "Username or password is incorrect";
                }
                return PartialView("~/Views/Home/Login.cshtml", responseFail);
            }
            
        }
        public IActionResult Login()
        {
            return View();
        }
    }
}