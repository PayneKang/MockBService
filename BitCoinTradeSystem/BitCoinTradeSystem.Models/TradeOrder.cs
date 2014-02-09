using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitCoinTradeSystem.Models
{
    public class TradeOrderCallback {
        public bool success { get; set; }
        public string IdentifyID { get; set; }
        public string errormessage { get; set; }
    }
    public class TradeOrderResponse {
        public string IdentifyID { get; set; }
        public List<TradeOrder> Orders { get; set; }
    }
    public partial class TradeOrder
    {
        public TradeOrder()
        {
            SellID = "0";
            BuyID = "0";
        }
        public string TradeType { get; set; }
        public float DealPrice { get; set; }
        public float DealQuantity { get; set; }
        public float DealAmount { get; set; }
        public float Profit { get; set; }
        public string SellID { get; set; }
        public string BuyID { get; set; }
        public float SellRequestPrice { get; set; }
        public float BuyRequestPrice { get; set; }
        public string DealTime { get; set; }
        public string SellRequestTime { get; set; }
        public string BuyRequestTime { get; set; }
    }
}