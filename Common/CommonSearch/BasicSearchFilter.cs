using Common.DataTransferObjects.CollectionPaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataTransferObjects.CommonSearch
{
    public class BasicSearchFilter : PagingParameter
    {
        public string SearchKeyword { get; set; }
        public IEnumerable<bool> Active { get; set; }

        public virtual string GetQueryString()
        {
            StringBuilder sb = new();

            sb.Append($"PageNumber={PageNumber}");
            sb.Append($"&PageSize={PageSize}");

            if (!string.IsNullOrEmpty(SearchKeyword))
            {
                sb.Append($"&SearchKeyword={SearchKeyword}");
            }

            if (Active != null && Active.Any())
            {
                foreach (bool active in Active)
                {
                    sb.Append($"&Active={active}");
                }
            }

            return sb.ToString();
        }
    }
}
