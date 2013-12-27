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

        public const string GET_HIGHESTBUY_INTERFACE = "http://localhost:50580/TradeData/GetHighestBuyRequest";
        public const string GET_LOWESTSELL_INTERFACE = "http://localhost:50580/TradeData/GetLowestSellRequest";

        public static Encoding ENCODING = Encoding.UTF8;
        public const int BUFFER_SIZE = 1024;

        public string ReadUrl(string url)
        {
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            Stream stream = httpResp.GetResponseStream();

            byte[] buffer = new byte[BUFFER_SIZE];
            int read = stream.Read(buffer, 0, BUFFER_SIZE);
            StringBuilder sb = new StringBuilder();
            while (read > 0)
            {
                sb.Append(ENCODING.GetString(buffer, 0, read));
                read = stream.Read(buffer, 0, BUFFER_SIZE);
            }
            stream.Close();
            return sb.ToString();
        }

        public TradeRequestItem GetHighestBuy()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TradeRequestItem req = serializer.Deserialize<TradeRequestItem>(ReadUrl(GET_HIGHESTBUY_INTERFACE));
            return req;
        }

        public TradeRequestItem GetLowestSell()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            TradeRequestItem req = serializer.Deserialize<TradeRequestItem>(ReadUrl(GET_LOWESTSELL_INTERFACE));
            return req;
        }
        
        public List<TradeOrder> Calculate()
        {
            lock (tradeLock)
            {
                TradeRequestItem buy = GetHighestBuy();
                TradeRequestItem sell = GetLowestSell();
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
                        sellorder.TradeType = "sell";
                        sellOrders.Add(sellorder);
                        sellReqs.Add(sell);
                        sell = GetLowestSell();
                    }
                    buyorder.BuyID = buy.TradeRequestID;
                    buyorder.BuyRequestPrice = buy.Price;
                    buyorder.DealQuantity = totalQuantity;
                    buyorder.TradeType = "buy";
                    
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
                        buyorder.TradeType = "buy";
                        buyOrders.Add(buyorder);
                        buyReqs.Add(buy);
                        buy = GetHighestBuy();
                    }
                    sellorder.SellID = sell.TradeRequestID;
                    sellorder.SellRequestPrice = sell.Price;
                    sellorder.DealQuantity = totalQuantity;
                    sellorder.TradeType = "sell";                    
                    sellOrders.Add(sellorder);
                }
                List<TradeOrder> orders = new List<TradeOrder>();
                orders.AddRange(sellOrders);
                orders.AddRange(buyOrders);
                return orders;
            }
        }
    }
}
