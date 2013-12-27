using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitCoinTradeSystem.Models
{
    public partial class TradeOrder
    {
        public string TradeType { get; set; }
        public float DealPrice { get; set; }
        public float DealQuantity { get; set; }
        public float DealAmount { get; set; }
        public float Profit { get; set; }
        public string SellID { get; set; }
        public string BuyID { get; set; }
        public float SellRequestPrice { get; set; }
        public float BuyRequestPrice { get; set; }
    }
}