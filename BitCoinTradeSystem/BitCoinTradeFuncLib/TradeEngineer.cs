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

                TradeOrderResponse response = new TradeOrderResponse();
                response.IdentifyID = identifyID;
                response.Orders = new List<TradeOrder>();
                TradeOrder order = new TradeOrder();
                if (sell != null)
                {
                    order.SellID = sell.TradeRequestID;
                    order.SellRequestPrice = sell.Price;
                    order.SellRequestTime = sell.RequestTime;
                }

                if (buy != null)
                {
                    order.BuyID = buy.TradeRequestID;
                    order.BuyRequestPrice = buy.Price;
                    order.BuyRequestTime = buy.RequestTime;
                }
                order.DealQuantity = 0;
                order.DealPrice = 0;
                order.DealAmount = 0;
                order.DealTime = DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY);
                order.TradeType = Constants.SELL_CODE;
                if (buy == null)
                {
                    response.Orders.Add(order);
                    return response;
                }
                if (sell == null)
                {
                    response.Orders.Add(order);
                    return response;
                }
                if (buy.Price < sell.Price)
                {
                    response.Orders.Add(order);
                    return response;
                }
                List<TradeOrder> orders = new List<TradeOrder>();
                List<TradeRequestItem> buyReqs = new List<TradeRequestItem>();
                List<TradeRequestItem> sellReqs = new List<TradeRequestItem>();
                buyReqs.Add(buy);
                sellReqs.Add(sell);
                float dealQuantity = buy.RemainQuantity > sell.RemainQuantity ? sell.RemainQuantity : buy.RemainQuantity;
                order.DealQuantity = dealQuantity;
                orders.Add(order);
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
