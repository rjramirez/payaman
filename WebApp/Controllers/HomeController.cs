using AutoMapper;
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
            //var userId = User.Identity.Name;
            return View();
            //if (userId != null)
            //    return View();
            //else
            //    return RedirectToAction("StatusPage", "Error", new { code = 401 });
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
        public async Task<IActionResult> Authenticate([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var authRequest = _mapper.Map<AuthenticateRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Authenticate", authRequest.GetStringContent());

            AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());
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