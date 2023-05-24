﻿using Common.Constants;
using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Common.DataTransferObjects.AppUserDetails;
using WebApp.Models.AppUser;
using IdentityModel.Client;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Authorize]
    public class AppUserController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AppUserController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllAppUsers()
        {
            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var token = ClaimService.GetClaimStringValue(User, "Token");
            client.SetBearerToken(token);

            var response = await client.GetAsync($"api/AppUser/GetAllAppUsers");

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<AppUserDetail> appUsers = JsonConvert.DeserializeObject<IEnumerable<AppUserDetail>>(await response.Content.ReadAsStringAsync());

                List<AppUserVM> appUserVM = appUsers.Select(au => new AppUserVM()
                {
                    AppUserId = au.AppUserId,
                    FirstName = au.FirstName,
                    LastName = au.LastName,
                    Username = au.Username,
                    AppUserRole = au.AppUserRole,
                    CreatedBy = au.CreatedBy,
                    CreatedDate = CommonService.GetPHTime(au.CreatedDate),
                    ModifiedBy = au.ModifiedBy,
                    ModifiedDate = CommonService.GetPHTime(au.ModifiedDate),
                }).ToList();

                return Ok(JsonConvert.SerializeObject(appUsers));
            }

            return new JsonResult(new { data = "" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AppUserDetail appUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            appUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.AppUserId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var token = ClaimService.GetClaimStringValue(User, "Token");
            client.SetBearerToken(token);

            HttpResponseMessage response = await client.PutAsync($"api/AppUser/Update", appUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Remove([FromBody] AppUserDetail appUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            appUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.AppUserId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var token = ClaimService.GetClaimStringValue(User, "Token");
            client.SetBearerToken(token);

            HttpResponseMessage response = await client.PostAsync($"api/AppUser/Remove", appUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (response.IsSuccessStatusCode)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AppUserDetail appUserDetail)
        {
            var ident = User.Identity as ClaimsIdentity;
            appUserDetail.TransactionBy = ident.Claims.FirstOrDefault(i => i.Type == ClaimConstant.AppUserId).Value;

            HttpClient client = _httpClientFactory.CreateClient("RITSApiClient");

            var token = ClaimService.GetClaimStringValue(User, "Token");
            client.SetBearerToken(token);

            HttpResponseMessage response = await client.PostAsync($"api/AppUser/Register", appUserDetail.GetStringContent());

            ClientResponse clientResponse = JsonConvert.DeserializeObject<ClientResponse>(await response.Content.ReadAsStringAsync());

            if (clientResponse.IsSuccessful)
                return Ok(clientResponse);
            else
                return BadRequest(clientResponse);
        }
    }
}