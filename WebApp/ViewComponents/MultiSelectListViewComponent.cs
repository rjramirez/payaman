using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models.SelectList;
using WebApp.Services.Interfaces;

namespace WebApp.ViewComponents
{
    public class MultiSelectListViewComponent : ViewComponent
    {
        private readonly ICommonService _commonService;
        public MultiSelectListViewComponent(ICommonService commonService)
        {
            _commonService = commonService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            string apiRoute,
            string controlId = "ddlSelectList",
            string optionLabel = "--Select--",
            IEnumerable<string> selectedValues = null,
            string additionalClasses = "",
            bool readOnly = false,
            bool enableCache = true)
        {
            IEnumerable<SelectListItem> selectListItems = await _commonService.GetReferenceDataSelectList(apiRoute, selectedValues, enableCache);

            SelectListViewModel selectListViewModel = new()
            {
                ControlID = controlId,
                ListItems = selectListItems,
                ReadOnly = readOnly,
                OptionLabel = optionLabel,
                AdditionalClasses = additionalClasses

            };

            return View("~/Views/SelectList/_MultiSelectList.cshtml", selectListViewModel);
        }
    }
}
