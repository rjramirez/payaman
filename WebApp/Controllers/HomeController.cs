using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using IdentityModel;
using Common.Constants;

namespace WebApp.Controllers
{
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
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult AuthenticationCallback()
        {
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

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
            }

        }


        [HttpPost]
        public async Task<IActionResult> Register([FromBody] AppUserDetail appUserDetail)
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var registerRequest = _mapper.Map<RegisterRequest>(appUserDetail);
            var response = await client.PostAsync($"api/User/Register", registerRequest.GetStringContent());

            if (response.IsSuccessStatusCode)
            {
                RegisterResponse authDetails = JsonConvert.DeserializeObject<RegisterResponse>(await response.Content.ReadAsStringAsync());

                var messageResponse = new
                {
                    IsCompleted = true,
                    Message = "Sign Up Successfully!"
                };

                return Ok(messageResponse);
            }
            else
            {
                var messageResponse = new
                {
                    IsCompleted = false,
                    Message = "Invalid Login"
                };
                return Ok(messageResponse);
            }

        }



        public IActionResult Login()
        {
            return View();
        }
    }
}