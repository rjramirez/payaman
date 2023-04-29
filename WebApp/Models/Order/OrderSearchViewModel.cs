using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Order;

namespace WebApp.Models.Order
{
    public class OrderSearchViewModel
    {
        public OrderSearchFilter OrderSearchFilter { get; set; }
        public PagedList<OrderSearchResult> OrderSearchResults { get; set; }
       
    }
}
