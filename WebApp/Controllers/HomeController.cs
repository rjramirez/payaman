﻿using AutoMapper;
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
using Azure;
using WebApp.Models.Receipt;

namespace WebApp.Controllers
{
    [Authorize]
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

        public IActionResult Index()
        {
            ViewBag.Title = "Dashboard";
            return View();
        }

        public IActionResult Products()
        {
            ViewBag.Title = "Products";
            return View();
        }

        public IActionResult Stores()
        {
            ViewBag.Title = "Stores";
            return View();
        }

        public IActionResult Orders()
        {
            ViewBag.Title = "Orders";
            return View();
        }

        public IActionResult Users()
        {
            ViewBag.Title = "Users";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromBody] OrderDetail orderDetail)
        {
            if (orderDetail.OrderItemList == null)
                return View();

            string storeSelectedCacheName = string.Format(CacheConstant.StoreSelectedCacheName, User.Identity.Name);
            ReceiptVM receiptVM = new();

            if (_memoryCache.TryGetValue(storeSelectedCacheName, out IEnumerable<ReferenceDataDetail> storeSelectedCached)) 
            {

                var ident = User.Identity as ClaimsIdentity;
                orderDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.EmployeeId).Value;

                string storeName = storeSelectedCached.SingleOrDefault(r => r.Name == CacheConstant.StoreNameCacheName).Value.ToString();
                string storeAddress = storeSelectedCached.SingleOrDefault(r => r.Name == CacheConstant.StoreAddressCacheName).Value.ToString();
                
                receiptVM.StoreName = storeName;
                receiptVM.StoreAddress = storeAddress;
                receiptVM.CashierName = User.Identity.Name;
                receiptVM.TotalAmount = orderDetail.TotalAmount;
                
                //Save the order
                HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");
                HttpResponseMessage response = await client.PostAsync($"api/Order/Add", orderDetail.GetStringContent());

                if (response.IsSuccessStatusCode)
                {
                    receiptVM.OrderItemList = orderDetail.OrderItemList;
                    receiptVM.OrderId = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());

                    //Save to memorycache
                    MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(_clientSetting.CacheExpirationMinutes));

                    string cartDetailsCacheName = string.Format(CacheConstant.CartDetailsCacheName, User.Identity.Name);
                    _memoryCache.Remove(cartDetailsCacheName);

                    _memoryCache.Set(cartDetailsCacheName, receiptVM, cacheEntryOptions);
                }

            }

            ClientResponse clientResponse = new ClientResponse()
            {
                Message = "",
                IsSuccessful = receiptVM.OrderItemList.Any() ? true : false,
                Data = receiptVM
            };

            return Ok(clientResponse);
        }

        public IActionResult Cart()
        {
            ViewBag.Title = "Cart";
            return View();
        }

        public IActionResult Receipt()
        {
            ViewBag.Title = "Receipt";

            string cartDetailsCacheName = string.Format(CacheConstant.CartDetailsCacheName, User.Identity.Name);
            if (_memoryCache.TryGetValue(cartDetailsCacheName, out ReceiptVM receiptVMCached))
            {
                _memoryCache.Remove(cartDetailsCacheName);

                return View(receiptVMCached);
            }

            ReceiptVM receiptVM = new ReceiptVM();

            return View(receiptVM);
        }

        public IActionResult Dashboard()
        {
            ViewBag.Title = "Dashboard";
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MenuRightBarStores(StoreVM storeVM)
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
                    Address = s.Address,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate
                }).ToList();

                string storeSelectedCacheName = string.Format(CacheConstant.StoreSelectedCacheName, User.Identity.Name);
                bool memCacheStoreIdAvailable = _memoryCache.TryGetValue(storeSelectedCacheName, out ReferenceDataDetail storeIdSelected);
                int storeIdFromMemCache = storeIdSelected != null ? Convert.ToInt32(storeIdSelected.Value) : 0;
                bool isStoreIdAlreadySet = false;

                foreach (StoreVM newStore in newStoreVM)
                {
                    //Store ID: If there is a selected store
                    if (storeVM.Id > 0)
                    {
                        //MemoryCache StoreID is available but not yet set. Override it.
                        if (memCacheStoreIdAvailable && !isStoreIdAlreadySet)
                        {
                            _memoryCache.Remove(storeSelectedCacheName);

                            //Set MemoryCache StoreIdSelected
                            List<ReferenceDataDetail> storeForMemoryCache = new List<ReferenceDataDetail> {
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreIdCacheName, Value = newStoreVM.First().Id },
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreNameCacheName, Value = newStoreVM.First().Name },
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreAddressCacheName, Value = newStoreVM.First().Address }
                            };

                            _memoryCache.Set(storeSelectedCacheName, storeForMemoryCache, cacheEntryOptions);
                            isStoreIdAlreadySet = true;
                        }

                        if (newStore.Id == storeVM.Id)
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
                            List<ReferenceDataDetail> storeForMemoryCache = new List<ReferenceDataDetail> {
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreIdCacheName, Value = newStoreVM.First().Id },
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreNameCacheName, Value = newStoreVM.First().Name },
                                new ReferenceDataDetail { Active = true, Name = CacheConstant.StoreAddressCacheName, Value = newStoreVM.First().Address }
                            };

                            _memoryCache.Set(storeSelectedCacheName, storeForMemoryCache, cacheEntryOptions);

                            newStore.IsSelected = true;
                        }
                        else
                            newStore.IsSelected = false;
                    }
                }

                return Ok(newStoreVM);
            }

            return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
        }

        public IActionResult AuthenticationCallback()
        {
            return RedirectToAction("Index", "Home");
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

        [AllowAnonymous]
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
                    new Claim(ClaimConstant.EmployeeId, authDetails.Id.ToString()),
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

        [AllowAnonymous]
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


        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
    }
}