using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeSystem.Models;
using System.Threading;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;

namespace BitCoinTradeFuncLib
{
    public class TradeEngineer
    {
        private static object tradeLock = new object();

        public const string GET_HIGHESTBUY_INTERFACE = "http://localhost:50580/TradeData/GetHighestBuyRequest?identifyID={0}";
        public const string GET_LOWESTSELL_INTERFACE = "http://localhost:50580/TradeData/GetLowestSellRequest?identifyID={0}";


        public string ReadUrl(string url,string identifyID)
        {
            return UrlReader.GetJsonResponse<string>(string.Format(url, identifyID));
        }

        public TradeRequestItem GetHighestBuy(string identifyID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TradeRequestItem req = serializer.Deserialize<TradeRequestItem>(ReadUrl(GET_HIGHESTBUY_INTERFACE, identifyID));
            return req;
        }

        public TradeRequestItem GetLowestSell(string identifyID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TradeRequestItem req = serializer.Deserialize<TradeRequestItem>(ReadUrl(GET_LOWESTSELL_INTERFACE,identifyID));
            return req;
        }
        
        public TradeOrderResponse Calculate(string identifyID)
        {
            lock (tradeLock)
            {
                TradeRequestItem buy = GetHighestBuy(identifyID);
                TradeRequestItem sell = GetLowestSell(identifyID);
                if (buy.Price < sell.Price)
                    return null;
                List<TradeOrder> buyOrders = new List<TradeOrder>();
                List<TradeOrder> sellOrders = new List<TradeOrder>();
                List<TradeRequestItem> buyReqs = new List<TradeRequestItem>();
                List<TradeRequestItem> sellReqs = new List<TradeRequestItem>();
                if (buy.Quantity > sell.Quantity)
                {
                    buyReqs.Add(buy);
                    TradeOrder buyorder = new TradeOrder();
                    float totalQuantity = 0f;
                    while (buy.Quantity > 0)
                    {
                        if (buy.Price < sell.Price)
                            break;
                        // 创建卖单
                        TradeOrder sellorder = new TradeOrder();
                        float dealQuantity = buy.Quantity > sell.Quantity ? sell.Quantity : buy.Quantity;
                        totalQuantity += dealQuantity;
                        buy.Quantity -= dealQuantity;
                        sell.Quantity -= dealQuantity;
                        sellorder.BuyID = buy.TradeRequestID;
                        sellorder.BuyRequestPrice = buy.Price;
                        //sellorder.DealAmount = dealQuantity * sell.Price;
                        //sellorder.DealPrice = sell.Price;
                        sellorder.DealQuantity = dealQuantity;
                        sellorder.SellID = sell.TradeRequestID;
                        sellorder.SellRequestPrice = sell.Price;
                        sellorder.TradeType = Consts.SELL_CODE;
                        sellOrders.Add(sellorder);
                        sellReqs.Add(sell);
                        sell = GetLowestSell(identifyID);
                    }
                    buyorder.BuyID = buy.TradeRequestID;
                    buyorder.BuyRequestPrice = buy.Price;
                    buyorder.DealQuantity = totalQuantity;
                    buyorder.TradeType = Consts.BUY_CODE;
                    
                    buyOrders.Add(buyorder);
                }
                else
                {
                    sellReqs.Add(sell);
                    TradeOrder sellorder = new TradeOrder();
                    float totalQuantity = 0f;
                    while (sell.Quantity > 0)
                    {
                        if (buy.Price < sell.Price)
                            break;
                        TradeOrder buyorder = new TradeOrder();
                        float dealQuantity = buy.Quantity > sell.Quantity ? sell.Quantity : buy.Quantity;
                        totalQuantity += dealQuantity;
                        buy.Quantity -= dealQuantity;
                        sell.Quantity -= dealQuantity;
                        buyorder.BuyID = buy.TradeRequestID;
                        buyorder.BuyRequestPrice = buy.Price;
                        //sellorder.DealAmount = dealQuantity * sell.Price;
                        //sellorder.DealPrice = sell.Price;
                        buyorder.DealQuantity = dealQuantity;
                        buyorder.SellID = sell.TradeRequestID;
                        buyorder.SellRequestPrice = sell.Price;
                        buyorder.TradeType = Consts.BUY_CODE;
                        buyOrders.Add(buyorder);
                        buyReqs.Add(buy);
                        buy = GetHighestBuy(identifyID);
                    }
                    sellorder.SellID = sell.TradeRequestID;
                    sellorder.SellRequestPrice = sell.Price;
                    sellorder.DealQuantity = totalQuantity;
                    sellorder.TradeType = Consts.SELL_CODE;                    
                    sellOrders.Add(sellorder);
                }
                List<TradeOrder> orders = new List<TradeOrder>();
                orders.AddRange(sellOrders);
                orders.AddRange(buyOrders);
                return new TradeOrderResponse()
                {
                    IdentifyID = identifyID,
                    Orders = orders
                };
            }
        }
    }
}
