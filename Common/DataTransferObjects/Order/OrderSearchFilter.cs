using Common.DataTransferObjects.CommonSearch;
using System.Text;

namespace Common.DataTransferObjects.Order
{
    public class OrderSearchFilter : BasicSearchFilter
    {
        public string Keyword { get; set; }
        public int StoreId { get; set; }
        public override string GetQueryString()
        {
            StringBuilder sb = new(base.GetQueryString());

            if (!string.IsNullOrEmpty(Keyword))
                sb.Append($"&Keyword={Keyword}");

            if (StoreId > 0)
                sb.Append($"&StoreId={StoreId}");

            return sb.ToString();
        }
    }
}