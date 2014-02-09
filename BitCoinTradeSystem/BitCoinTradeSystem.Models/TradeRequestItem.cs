using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitCoinTradeSystem.Models
{
    public partial class TradeRequestItem
    {
        public const string BUY = "buy";
        public const string SELL = "sell";

        public string Type { get; set; }
        public float Price { get; set; }
        public float Quantity { get; set; }
        public float RemainQuantity { get; set; }
        public float Amount { get { return Price * Quantity; } }
        public string TradeRequestID { get; set; }
        public string RequestTime { get; set; }
        public static List<TradeRequestItem> TestBuyList
        {
            get
            {
                return new List<TradeRequestItem>()
                {
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5431.34f,
                          Quantity = 5.4f,
                          RemainQuantity = 5.4f,
                           TradeRequestID="004"
                    },
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5430.24f,
                          Quantity = 10.4f,
                          RemainQuantity = 10.4f,
                           TradeRequestID="005"
                    },
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5428.34f,
                          Quantity = 0.4f,
                          RemainQuantity = 0.4f,
                           TradeRequestID="006"
                    },
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5427.34f,
                          Quantity = 8.43f,
                          RemainQuantity = 8.43f,
                           TradeRequestID="007"
                    },
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5425f,
                          Quantity = 3.4f,
                          RemainQuantity = 3.4f,
                           TradeRequestID="008"
                    },
                    new TradeRequestItem(){
                        Type = TradeRequestItem.BUY,
                         Price = 5424.34f,
                          Quantity = 8.4f,
                          RemainQuantity = 8.4f,
                           TradeRequestID="009"
                    }
                };
            }
        }

        public static List<TradeRequestItem> TestSellList
        {
            get
            {
                return new List<TradeRequestItem>()
                {
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5422.08f,
                         Quantity = 0.8f,
                         RemainQuantity = 0.8f,
                           TradeRequestID="001"
                    },
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5422.48f,
                         Quantity = 1.8f,
                         RemainQuantity = 1.8f,
                           TradeRequestID="002"
                    },
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5423.14f,
                         Quantity = 3.4f,
                         RemainQuantity = 3.4f,
                           TradeRequestID="003"
                    },
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5426.34f,
                         Quantity = 9.8f,
                         RemainQuantity = 9.8f,
                           TradeRequestID="010"
                    },
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5438.08f,
                         Quantity = 5.2f,
                         RemainQuantity = 5.2f,
                           TradeRequestID="011"
                    },
                    new TradeRequestItem(){
                         Type = TradeRequestItem.SELL,
                         Price = 5440.08f,
                         Quantity = 1.8f,
                         RemainQuantity = 1.8f,
                         TradeRequestID="012"
                    }
                };
            }
        }
    }
}