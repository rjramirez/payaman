using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.CommonSearch;
using Common.DataTransferObjects.ErrorLog;

namespace WebApp.Models.Error
{
    public class ErrorSearchViewModel
    {
        public CommonSearchFilter CommonSearchFilter { get; set; }
        public PagedList<ErrorLogDetail> ErrorLogDetails { get; set; }
    }
}
