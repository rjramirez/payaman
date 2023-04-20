using Common.DataTransferObjects.CollectionPaging;

namespace Common.DataTransferObjects.CommonSearch
{
    public class CommonSearchFilter : PagingParameter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchKeyword { get; set; }
    }
}
