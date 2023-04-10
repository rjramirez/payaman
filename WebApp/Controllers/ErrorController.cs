using Common.Constants;
using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;
using Common.DataTransferObjects.Version;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using WebApp.Extensions;
using WebApp.Models.Error;

namespace WebApp.Controllers
{
    [Authorize(Policy = "Support")]
    public class ErrorController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ErrorController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return RedirectToAction("StatusPage", "Error", new { code = 403 });
        }

        [HttpGet]
        public async Task<IActionResult> Search(CommonSearchFilter commonSearchFilter)
        {
            string url = $"api/ErrorLog/ErrorLogListPagedSearch?StartDate={commonSearchFilter.StartDate}&EndDate={commonSearchFilter.EndDate}&" +
                     $"PageNumber={commonSearchFilter.PageNumber}&PageSize={commonSearchFilter.PageSize}&" +
                     $"SearchKeyword={commonSearchFilter.SearchKeyword}";

            HttpClient httpClient = _httpClientFactory.CreateClient("ProjectTemplateApiClient");
            HttpResponseMessage response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ErrorLogDetail> errorLogDetails = JsonConvert.DeserializeObject<IEnumerable<ErrorLogDetail>>(await response.Content.ReadAsStringAsync());
                PagingMetadata pagingMetadata = JsonConvert.DeserializeObject<PagingMetadata>(response.Headers.GetValues(PagingConstant.PagingHeaderKey).FirstOrDefault());
                PagedList<ErrorLogDetail> result = new(errorLogDetails.ToList(), pagingMetadata)
                {
                    PageClickEvent = "ErrorLog.changePage({0})"
                };

                ErrorSearchViewModel errorSearchViewModel = new()
                {
                    CommonSearchFilter = commonSearchFilter,
                    ErrorLogDetails = result
                };

                return View("Views/Error/ErrorLogSearch.cshtml", errorSearchViewModel);
            }

            return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            HttpClient client = _httpClientFactory.CreateClient("ProjectTemplateApiClient");
            var response = await client.GetAsync($"api/ErrorLog/{id}");

            if (response.IsSuccessStatusCode)
            {
                ErrorLogDetail errorLogDetail = JsonConvert.DeserializeObject<ErrorLogDetail>(await response.Content.ReadAsStringAsync());
                return View(errorLogDetail);
            }
            else
            {
                return RedirectToAction("StatusPage", "Error", await response.GetErrorMessage());
            }
        }

        [AllowAnonymous]
        public IActionResult StatusPage(ErrorMessage errorMessage, int code)
        {
            StatusPageViewModel model;
            if (string.IsNullOrEmpty(errorMessage.Type) && code != 0)
            {
                model = new(code, User.Identity.Name);
            }
            else
            {
                model = new(errorMessage);
            }

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LogError()
        {
            string user = User.Identity.Name;

            HttpClient client = _httpClientFactory.CreateClient("ProjectTemplateApiClient");
            IExceptionHandlerPathFeature context = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            SaveErrorLog saveErrorLog = new()
            {
                Message = context.Error.InnerException != null
                    ? $"{context.Error.Message} | {context.Error.InnerException.Message}"
                    : context.Error.Message,

                StackTraceId = HttpContext.TraceIdentifier,
                StackTrace = context.Error.StackTrace,
                Path = context.Path,
                DateCreated = DateTime.Now,
                UserIdentity = user ?? "UnAuthenticated User",
                Source = context.Error.Source,
                BuildVersion = VersionDetail.DisplayVersion()
            };

            await client.PostAsync($"api/ErrorLog", saveErrorLog.GetStringContent());

            Request.Headers.TryGetValue("x-requested-with", out StringValues requestedWith);
            if (requestedWith.Equals("XMLHttpRequest"))
            {
                if (context.Error.GetType() == typeof(ArgumentException))
                {
                    ErrorMessage errorMessage = new(saveErrorLog.StackTraceId, ErrorMessageTypeConstant.ArgumentException, saveErrorLog.Message);
                    return BadRequest(errorMessage);
                }
                else
                {
                    ErrorMessage errorMessage = new(saveErrorLog.StackTraceId, ErrorMessageTypeConstant.InternalServerException, null);
                    return BadRequest(errorMessage);
                }
            }

            if (context.Error.GetType() == typeof(ArgumentException))
            {
                ErrorMessage errorMessage = new(saveErrorLog.StackTraceId, ErrorMessageTypeConstant.ArgumentException, saveErrorLog.Message);
                return RedirectToAction("StatusPage", "Error", errorMessage);
            }
            else
            {
                ErrorMessage errorMessage = new(saveErrorLog.StackTraceId, ErrorMessageTypeConstant.InternalServerException, null);
                return RedirectToAction("StatusPage", "Error", errorMessage);
            }
        }
    }
}
