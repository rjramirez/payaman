using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApp.Models.SelectList
{
    public class SelectListViewModel
    {
        public string ControlID { get; set; }
        public IEnumerable<SelectListItem> ListItems { get; set; }
        public bool ReadOnly { get; set; }
        public string OptionLabel { get; set; }
        public string AdditionalClasses { get; set; }
    }
}
