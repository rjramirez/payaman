using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataTransferObjects.Configurations
{
    public class WebAppSetting
    {
        public int CacheExpirationMinutes { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public int SessionExpirationMinutes { get; set; }
        public int BulkTransferMinimunItems { get; set; }
    }
}
