using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CanonWebApp.Code
{
    [Serializable()]
    public class MainMonitorFilter
    {
        public List<int> Categories { get; set; }
        public List<int> Shops { get; set; }
        public int ChannelType { get; set; }
        public string ProductFilter { get; set; }
        public int PriceType { get; set; }
        public int Condition { get; set; }
        public int PageSize { get; set; }
    }
}
