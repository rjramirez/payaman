namespace Common.DataTransferObjects.CollectionPaging
{
    public class PagingParameter
    {
        const int maxPageSize = 100000;
        private int pageSize = 10;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }
}
