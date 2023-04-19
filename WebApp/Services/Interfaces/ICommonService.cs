using Common.DataTransferObjects.ReferenceData;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Principal;

namespace WebApp.Services.Interfaces
{
    public interface ICommonService
    {
        string ConvertBoleanToYesOrNo(bool bolean);
        Task<IEnumerable<ReferenceDataDetail>> GetReferenceDataFromApi(string apiEndpoint, bool enableCache);
        Task<IEnumerable<SelectListItem>> GetReferenceDataSelectList(string apiEndpoint, string selectedValue, bool enableCache);
        Task<IEnumerable<SelectListItem>> GetReferenceDataSelectList(string apiEndpoint, IEnumerable<string> selectedValues, bool enableCache);
        Task<ReferenceDataDetail> GetUserRole(IPrincipal principal);
    }
}
