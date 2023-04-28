using Common.DataTransferObjects.CommonSearch;
using System.Text;

namespace Common.DataTransferObjects.Order
{
    public class OrderSearchFilter : BasicSearchFilter
    {
        public string Keyword { get; set; }
        public string Price { get; set; }

        public override string GetQueryString()
        {
            StringBuilder sb = new(base.GetQueryString());

            if (!string.IsNullOrEmpty(Keyword))
                sb.Append($"&Keyword={Keyword}");

            if (!string.IsNullOrEmpty(Price))
                sb.Append($"&Price={Price}");

            return sb.ToString();
        }
    }
}