using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
using Common.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.ReferenceData;
using WebApp.Extensions;
using Common.DataTransferObjects.Order;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Models.Order;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public HomeController(IHttpClientFactory httpClientFactory, IMapper mapper, IMemoryCache memoryCache, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        [Authorize(Policy="Admin")]
        public IActionResult Index()
        {
            ViewBag.Title = "Dashboard";
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Products()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Stores()
        {
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Orders()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(OrderSearchFilter orderSearchFilter)
        {
            //Get the role of the current user of Search
            //string userRoleCacheName = string.Format(RoleConstant.UserRoleCacheName, User.Identity.Name);
            //if (_memoryCache.TryGetValue(userRoleCacheName, out ReferenceDataDetail userRole))
            //{
            //    if (Convert.ToInt32(userRole.Value) == AppRoleConstant.OPSLV1 ||
            //        Convert.ToInt32(userRole.Value) == AppRoleConstant.OPSLV2)
            //    {
            //        string msGraphEmployeeCacheName = string.Format(RoleConstant.MsGraphEmployeeCacheName, User.Identity.Name);
            //        if (_memoryCache.TryGetValue(msGraphEmployeeCacheName, out IEnumerable<ReferenceDataDetail> msGraphEmployeeCached))
            //        {
            //            string loggedInEmployeeId = msGraphEmployeeCached.SingleOrDefault(r => r.Name == "MMTEmployeeId").Value.ToString();
            //        }
            //    }
            //}

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Order/Search?{orderSearchFilter.GetQueryString()}");
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<OrderSearchResult> orderSearchResult = JsonConvert.DeserializeObject<IEnumerable<OrderSearchResult>>(await response.Content.ReadAsStringAsync());
                PagingMetadata pagingMetadata = JsonConvert.DeserializeObject<PagingMetadata>(response.Headers.GetValues(PagingConstant.PagingHeaderKey).FirstOrDefault());
                PagedList<OrderSearchResult> result = new(orderSearchResult.ToList(), pagingMetadata)
                {
                    PageClickEvent = "OrderSearch.changePage({0})"
                };

                OrderSearchViewModel orderSearchViewModel = new()
                {
                    OrderSearchFilter = orderSearchFilter,
                    OrderSearchResults = result
                };

                //await _notificationHubContext.Clients.All.SendAsync(NotificationConstant.RefreshNotification);

                return View("Views/Home/Search.cshtml", orderSearchViewModel);
            }

            return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
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
                    new Claim("name", authDetails.FirstName + " " + authDetails.LastName),
                    new Claim(ClaimConstant.ClientId, authDetails.Username),
                    new Claim("Token", authDetails.Token),
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