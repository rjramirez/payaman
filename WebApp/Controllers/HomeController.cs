using AutoMapper;
using Common.DataTransferObjects;
using Common.DataTransferObjects.AppUserDetails;
using Common.DataTransferObjects.ErrorLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    [Authorize]
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
            var userId = User.Identity.Name;
            if (userId != null)
                return View();
            else
                return RedirectToAction("StatusPage", "Error", new { code = 401 });
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AuthenticationCallback()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var loginRequest = _mapper.Map<LoginRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Login", loginRequest.GetStringContent());

            string authResponse = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            MessageResponse message = JsonConvert.DeserializeObject<MessageResponse>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                message.IsCompleted = true;
                return Ok(message);
            }
            else
            {
                message.IsCompleted = false;
                return Ok(message);
            }

        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var loginRequest = _mapper.Map<LoginRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Register", loginRequest.GetStringContent());

            string authResponse = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());
            MessageResponse message = JsonConvert.DeserializeObject<MessageResponse>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                message.IsCompleted = true;
                return Ok(message);
            }
            else
            {
                message.IsCompleted = false;
                return Ok(message);
            }

        }



        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
    }
}