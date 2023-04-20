using Common.DataTransferObjects.CollectionPaging;
using Common.DataTransferObjects.Employee;

namespace WebApp.Models.Product
{
    public class ProductVM
    {
        public ProductSearchFilter ProductSearchFilter { get; set; }
        public PagedList<ProductSearchResult> ProductSearchResults { get; set; }
    }
}
