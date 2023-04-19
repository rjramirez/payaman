using AutoMapper;
using Common.DataTransferObjects;
using Common.DataTransferObjects.AppUserDetails;
<<<<<<< HEAD
using Common.DataTransferObjects.ErrorLog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
=======
using Common.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using IdentityModel;
using Common.Constants;
>>>>>>> dev

namespace WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;

		public HomeController(IHttpClientFactory httpClientFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Policy="Admin")]
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

<<<<<<< HEAD
=======
        public IActionResult Dashboard()
        {
            return View();
        }

>>>>>>> dev
        public IActionResult AuthenticationCallback()
        {
            return RedirectToAction("Index", "Home");
        }

<<<<<<< HEAD
        [AllowAnonymous]
=======
>>>>>>> dev
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

<<<<<<< HEAD
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
=======
            var loginRequest = _mapper.Map<AuthenticateRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Authenticate", loginRequest.GetStringContent());

            AuthenticateResponse authResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());
            if (response.IsSuccessStatusCode)
            {
                AuthenticateResponse authDetails = JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());

                // Create a new ClaimsIdentity with the desired claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.PrimarySid, authDetails.Id.ToString()),
                    new Claim(ClaimTypes.Name, authDetails.Username),
                    new Claim(ClaimConstant.ClientId, authDetails.Username),
                    new Claim(ClaimTypes.Role, authDetails.Role.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { IsPersistent = true };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                var messageResponse = new
                {
                    IsCompleted = true,
                    Message = authResponse.Token
                };
                return Ok(messageResponse);
            }
            else
            {
                var messageResponse = new
                {
                    IsCompleted = false,
                    Message = authResponse.Token
                };

                return Ok(messageResponse);
>>>>>>> dev
            }

        }


<<<<<<< HEAD
        [AllowAnonymous]
=======
>>>>>>> dev
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var registerRequest = _mapper.Map<RegisterRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Register", registerRequest.GetStringContent());

<<<<<<< HEAD
            MessageResponse messageResponse = new();
            if (response.IsSuccessStatusCode)
            {
                string message = JsonConvert.DeserializeObject<string>(await response.Content.ReadAsStringAsync());

                messageResponse.IsCompleted = true;
                messageResponse.Message = message;
=======
            if (response.IsSuccessStatusCode)
            {
                RegisterResponse authDetails = JsonConvert.DeserializeObject<RegisterResponse>(await response.Content.ReadAsStringAsync());

                var messageResponse = new
                {
                    IsCompleted = true,
                    Message = "Sign Up Successfully!"
                };

>>>>>>> dev
                return Ok(messageResponse);
            }
            else
            {
<<<<<<< HEAD
                messageResponse.IsCompleted = false;
=======
                var messageResponse = new
                {
                    IsCompleted = false,
                    Message = "Invalid Login"
                };
>>>>>>> dev
                return Ok(messageResponse);
            }

        }



<<<<<<< HEAD
        [AllowAnonymous]
=======
>>>>>>> dev
        public IActionResult Login()
        {
            return View();
        }
    }
}