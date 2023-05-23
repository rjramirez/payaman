using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services.Interfaces;
using WebApp.Models.SelectList;

namespace WebApp.ViewComponents
{
    public class SelectListViewComponent : ViewComponent
    {
        private readonly ICommonService _commonService;
        public SelectListViewComponent(ICommonService commonService)
        {
            _commonService = commonService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string apiRoute,
            string controlId = "ddlSelectList",
            string optionLabel = "--Select--",
            string selectedValue = null,
            string additionalClasses = "",
            bool readOnly = false,
            bool enableCache = true)
        {
            IEnumerable<SelectListItem> selectListItems = await _commonService.GetReferenceDataSelectList(apiRoute, selectedValue, enableCache);

            SelectListViewModel selectListViewModel = new()
            {
                ControlID = controlId,
                ListItems = selectListItems,
                ReadOnly = readOnly,
                OptionLabel = optionLabel,
                AdditionalClasses = additionalClasses
            };

            return View("~/Views/SelectList/_SelectList.cshtml", selectListViewModel);
        }
    }
}
