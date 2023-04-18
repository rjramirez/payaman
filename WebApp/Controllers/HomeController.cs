﻿using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using WebApp.Authorization;

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

        [Authorize(Role.Admin)]
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
                    new Claim(ClaimTypes.Name, authDetails.Username),
                    new Claim(ClaimTypes.Role, authDetails.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, "User");

                // Create a new ClaimsPrincipal with the custom identity
                var principal = new ClaimsPrincipal(identity);

                // Set the HttpContext.User property to the custom principal
                _httpContextAccessor.HttpContext.User = principal;

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
                AuthenticateResponse authDetails = JsonConvert.DeserializeObject<AuthenticateResponse>(await response.Content.ReadAsStringAsync());


                // Create a new ClaimsIdentity with the desired claims
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, authDetails.Username),
                    new Claim(ClaimTypes.Role, authDetails.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, "User");

                // Create a new ClaimsPrincipal with the custom identity
                var principal = new ClaimsPrincipal(identity);

                // Set the HttpContext.User property to the custom principal
                _httpContextAccessor.HttpContext.User = principal;

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