using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeSystem.Models;
using System.Threading;
using System.Web.Script.Serialization;
using System.Net;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using CommonLib;

namespace BitCoinTradeFuncLib
{
    public class TradeEngineer
    {
        private static object tradeLock = new object();

        public static string GET_HIGHESTBUY_INTERFACE { get; private set; }
        public static string GET_LOWESTSELL_INTERFACE { get; private set; }
        public static string CALLBACK_INTERFACE { get; private set; }
        public static int ORDER_INTERVAL { get; private set; }

        static TradeEngineer()
        {
            GET_HIGHESTBUY_INTERFACE = ConfigurationManager.AppSettings["GetHighestBuy"];
            GET_LOWESTSELL_INTERFACE = ConfigurationManager.AppSettings["GetLowestSell"];
            CALLBACK_INTERFACE = ConfigurationManager.AppSettings["Callback"];
            ORDER_INTERVAL = int.Parse(ConfigurationManager.AppSettings["OrderInterval"]);
        }


        public string ReadUrl(string url,string identifyID)
        {
            return UrlReader.GetJsonResponse<string>(string.Format(url, identifyID));
        }

        public TradeRequestItem GetHighestBuy(string identifyID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ParameterValue[] paras = new ParameterValue[] { 
                new ParameterValue(){ Name="identifyID", Value=identifyID}
            };
            TradeRequestItem req = UrlReader.GetJsonResponse<TradeRequestItem>(GET_HIGHESTBUY_INTERFACE, SendType.Get, paras);
            return req;
        }

        public TradeRequestItem GetLowestSell(string identifyID)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ParameterValue[] paras = new ParameterValue[] { 
                new ParameterValue(){ Name="identifyID", Value=identifyID}
            };
            try
            {
                TradeRequestItem req = UrlReader.GetJsonResponse<TradeRequestItem>(GET_LOWESTSELL_INTERFACE, SendType.Get, paras);
                return req;
            }
            catch (Exception ex)
            {
                return null;
            }
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
                        if (sell == null)
                            break;
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
                        sellorder.TradeType = Constants.SELL_CODE;
                        sellorder.DealTime = DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY);
                        sellOrders.Add(sellorder);
                        sellReqs.Add(sell);
                        sell = GetLowestSell(identifyID);
                    }
                    buyorder.BuyID = buy.TradeRequestID;
                    buyorder.BuyRequestPrice = buy.Price;
                    buyorder.DealQuantity = totalQuantity;
                    buyorder.TradeType = Constants.BUY_CODE;
                    buyorder.DealTime = DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY);
                    buyOrders.Add(buyorder);
                }
                else
                {
                    sellReqs.Add(sell);
                    TradeOrder sellorder = new TradeOrder();
                    float totalQuantity = 0f;
                    while (sell.Quantity > 0)
                    {
                        if (buy == null)
                            break;
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
                        buyorder.TradeType = Constants.BUY_CODE;
                        buyorder.DealTime = DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY);
                        buyOrders.Add(buyorder);
                        buyReqs.Add(buy);
                        buy = GetHighestBuy(identifyID);
                    }
                    sellorder.SellID = sell.TradeRequestID;
                    sellorder.SellRequestPrice = sell.Price;
                    sellorder.DealQuantity = totalQuantity;
                    sellorder.TradeType = Constants.SELL_CODE;
                    sellorder.DealTime = DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY);
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

        public TradeOrderCallback SendOrderToResponse(TradeOrderResponse orderresponse)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ParameterValue[] paras = new ParameterValue[] { 
                new ParameterValue(){ Name="identifyID", Value=orderresponse.IdentifyID},
                new ParameterValue(){Name="jsonStr",Value = serializer.Serialize(orderresponse.Orders)}
            };
            Debug.Print(serializer.Serialize(orderresponse.Orders));
            TradeOrderCallback result = UrlReader.GetJsonResponse<TradeOrderCallback>(CALLBACK_INTERFACE,SendType.Post,paras);
            return result;
        }
    }
}
