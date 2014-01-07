using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitCoinTradeSystem.Models
{
    public class KLineCallback
    {
        public bool success { get; set; }
        public string IdentifyID { get; set; }
        public string errormessage { get; set; }
    }
    public class KLineResponse
    {
        public string IdentifyID { get; set; }
        public List<KLineItem> KLines { get; set; }
    }
}
