using AutoMapper;
using Common.DataTransferObjects.AppUserDetails;
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
using Common.DataTransferObjects.Store;
using WebApp.Models.Store;
using Common.DataTransferObjects.AppSettings;
using WebApp.Models.Cart;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private IMapper _mapper;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly ClientSetting _clientSetting;

        public HomeController(IHttpClientFactory httpClientFactory, IMapper mapper, IMemoryCache memoryCache, 
            IHttpContextAccessor httpContextAccessor, ClientSetting clientSetting)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _clientSetting = clientSetting;
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
            ViewBag.Title = "Products";
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Stores()
        {
            ViewBag.Title = "Stores";
            return View();
        }

        [Authorize(Policy = "Admin")]
        public IActionResult Orders()
        {
            ViewBag.Title = "Orders";
            return View();
        }
        public IActionResult ViewCart(OrderDetail orderDetail)
        {
            ViewBag.Title = "View Cart";

            string storeSelectedCacheName = string.Format(RoleConstant.StoreSelectedCacheName, User.Identity.Name);
            CartVM cartVM = new();

            if (_memoryCache.TryGetValue(storeSelectedCacheName, out ReferenceDataDetail storeIdSelected)) 
            {
                cartVM.StoreName = storeIdSelected.Name;
                cartVM.CashierName = User.Identity.Name;
                //Get the next temporary orderId, storename, storeaddress

            }

            return View(cartVM);
        }
        public IActionResult Cart()
        {
            ViewBag.Title = "Cart";
            return View();
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MenuRightBarStores(int storeId)
         {
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));
            
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Store/GetAllStores");
            
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<StoreDetail> storesAvailable = JsonConvert.DeserializeObject<IEnumerable<StoreDetail>>(await response.Content.ReadAsStringAsync());
                List<StoreVM> newStoreVM = storesAvailable.Select(s => new StoreVM()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Image = s.Image,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate
                }).ToList();

                string storeSelectedCacheName = string.Format(RoleConstant.StoreSelectedCacheName, User.Identity.Name);
                bool memCacheStoreIdAvailable = _memoryCache.TryGetValue(storeSelectedCacheName, out ReferenceDataDetail storeIdSelected);
                int storeIdFromMemCache = storeIdSelected != null ? Convert.ToInt32(storeIdSelected.Value) : 0;
                bool isStoreIdAlreadySet = false;

                foreach (StoreVM newStore in newStoreVM)
                {
                    //Store ID: If there is a selected store
                    if (storeId > 0)
                    {
                        //MemoryCache StoreID is available but not yet set. Override it.
                        if (memCacheStoreIdAvailable && !isStoreIdAlreadySet)
                        {
                            _memoryCache.Remove(storeSelectedCacheName);

                            //Set MemoryCache StoreIdSelected
                            ReferenceDataDetail storeIdSelectedForMemoryCache = new ReferenceDataDetail { Active = true, Name = "StoreIdSelected", Value = storeId };
                            _memoryCache.Set(storeSelectedCacheName, storeIdSelectedForMemoryCache, cacheEntryOptions);
                            isStoreIdAlreadySet = true;
                        }

                        if (newStore.Id == storeId)
                            newStore.IsSelected = true;
                        else
                            newStore.IsSelected = false;
                    }
                    //Store ID: MemCached StoreID is available.
                    else if (memCacheStoreIdAvailable && storeIdFromMemCache > 0)
                    {
                        if (newStore.Id == storeIdFromMemCache)
                            newStore.IsSelected = true;
                        else
                            newStore.IsSelected = false;
                    }
                    else
                    {
                        //Default
                        if (newStoreVM.First().Id == newStore.Id)
                        {
                            //Set MemoryCache StoreIdSelected
                            ReferenceDataDetail storeIdSelectedForMemoryCache = new ReferenceDataDetail { Active = true, Name = "StoreIdSelected", Value = newStore.Id };
                            _memoryCache.Set(storeSelectedCacheName, storeIdSelectedForMemoryCache, cacheEntryOptions);

                            newStore.IsSelected = true;
                        }
                        else
                            newStore.IsSelected = false;
                    }
                }

                return PartialView("~/Views/Menu/_LRNBLinks.cshtml", newStoreVM);
            }

            return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
        }

        [HttpGet]
        public async Task<IActionResult> Search(OrderSearchFilter orderSearchFilter)
        {
            //string storeSelectedCacheName = string.Format(RoleConstant.StoreSelectedCacheName, User.Identity.Name);
            //if (_memoryCache.TryGetValue(storeSelectedCacheName, out ReferenceDataDetail storeIdSelected)) 
            //{
            //    int storeIdFromCache = Convert.ToInt32(storeIdSelected.Value);
            //    orderSearchFilter.StoreId = storeId > 0 ? storeId : 0;
            //}


            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
            HttpResponseMessage response = await client.GetAsync($"api/Order/Search?{orderSearchFilter.GetQueryString()}");
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<OrderSearchResult> orderSearchResult = JsonConvert.DeserializeObject<IEnumerable<OrderSearchResult>>(await response.Content.ReadAsStringAsync());
                PagingMetadata pagingMetadata = JsonConvert.DeserializeObject<PagingMetadata>(response.Headers.GetValues(PagingConstant.PagingHeaderKey).FirstOrDefault());
                PagedList<OrderSearchResult> result = new(orderSearchResult.ToList(), pagingMetadata)
                {
                    PageClickEvent = "Dashboard.changePage({0})"
                };

                OrderSearchViewModel orderSearchViewModel = new()
                {
                    OrderSearchFilter = orderSearchFilter,
                    OrderSearchResults = result
                };

                //await _notificationHubContext.Clients.All.SendAsync(NotificationConstant.RefreshNotification);

                return PartialView("~/Views/Dashboard/_Dashboard_Orders.cshtml", orderSearchViewModel);
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
                    new Claim("UserGivenName", authDetails.FirstName + " " + authDetails.LastName),
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