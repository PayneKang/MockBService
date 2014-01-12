using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using BitCoinTradeSystem.Models;
using System.Configuration;
using CommonLib;

namespace BitCoinTradeFuncLib
{
    public class KLineEngineer
    {
        public static string GET_LATESTKLINETIME_INTERFACE { get; private set; }
        public static string GET_ORDERLIST_INTERFACE { get; private set; }
        public static string CALLBACK_INTERFACE { get; private set; }
        public static string GET_LATESTKLINE_INTERFACE { get; private set; }
        public static string GET_SERVERTIME_INTERFACE { get; private set; }
        public static int KLINE_INTERVAL { get; private set; }
        public static int MINUTES_PER_TIME { get; private set; }
        public static int KLINE_MINUTETYPE { get; private set; }
        static KLineEngineer()
        {
            GET_LATESTKLINETIME_INTERFACE = ConfigurationManager.AppSettings["GetLatestKLineTimeInterface"];
            GET_ORDERLIST_INTERFACE = ConfigurationManager.AppSettings["GetOrderListInterface"];
            CALLBACK_INTERFACE = ConfigurationManager.AppSettings["KLineCallback"];
            GET_LATESTKLINE_INTERFACE = ConfigurationManager.AppSettings["GetLatestKLine"];
            GET_SERVERTIME_INTERFACE = ConfigurationManager.AppSettings["GetServerTimeInterface"];
            KLINE_INTERVAL = int.Parse(ConfigurationManager.AppSettings["KLineInterval"]);
            MINUTES_PER_TIME = int.Parse(ConfigurationManager.AppSettings["MinutesPerKLine"]);
            KLINE_MINUTETYPE = int.Parse(ConfigurationManager.AppSettings["KLineMinutes"]);
        }
        public KLineCallback SendKLines(KLineResponse kline)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            ParameterValue[] paras = new ParameterValue[] { 
                new ParameterValue(){ Name="identifyID", Value=kline.IdentifyID},
                new ParameterValue(){Name="jsonStr",Value = serializer.Serialize(kline.KLines)}
            };
            KLineCallback result = UrlReader.GetJsonResponse<KLineCallback>(CALLBACK_INTERFACE, SendType.Post, paras);
            return result;
        }
        private DateTime GetTheLatestDateTime()
        {
            string returnStr = UrlReader.ReadUrl(GET_LATESTKLINETIME_INTERFACE).Replace("\"","");
            return Utils.ParseDateTime(returnStr, Constants.DATEFORMAT_NUMONLY);
        }
        private DateTime GetServerDateTime()
        {
            string returnStr = UrlReader.ReadUrl(GET_SERVERTIME_INTERFACE).Replace("\"", "");
            return Utils.ParseDateTime(returnStr, Constants.DATEFORMAT_NUMONLY);
        }
        private List<TradeOrder> GetTradeOrder(DateTime startTime, DateTime toTime, string orderType, string identifyID)
        {
            string url = string.Format(GET_ORDERLIST_INTERFACE, startTime.ToString(Constants.DATEFORMAT_NUMONLY), toTime.ToString(Constants.DATEFORMAT_NUMONLY), identifyID);
            return UrlReader.GetJsonResponse<List<TradeOrder>>(url);
        }
        public KLineResponse Calculate(string identifyID)
        {
            DateTime startTime = GetTheLatestDateTime();
            DateTime serverTime = GetServerDateTime();
            DateTime toTime = startTime.AddMinutes(MINUTES_PER_TIME);
            if (toTime > serverTime)
                toTime = serverTime;
            List<TradeOrder> orders = GetTradeOrder(startTime, toTime, Constants.BUY_CODE, identifyID);
            KLineItem latestKLine = GetLatestKLine(startTime);
            List<KLineItem> result = new List<KLineItem>();
            result = CalculateKLine(orders, latestKLine, KLINE_MINUTETYPE, startTime, toTime);
            return new KLineResponse()
            {
                IdentifyID = identifyID,
                StartTime = startTime.ToString(Constants.DATEFORMAT_NUMONLY),
                EndTime = result.Max(x=>x.KLineTimeString),
                KLines = result
            };
        }
        private KLineItem GetLatestKLine(DateTime beforeTime)
        {
            return UrlReader.GetJsonResponse<KLineItem>(GET_LATESTKLINE_INTERFACE,SendType.Get, new ParameterValue[]{
                new ParameterValue(){
                     Name = "beforeTime",
                     Value = beforeTime.ToString(Constants.DATEFORMAT_NUMONLY)
                }
            });
        }
        private List<KLineItem> CalculateKLine(List<TradeOrder> orderList,KLineItem latestKLine, int intervalMinutes, DateTime startTime,DateTime endTime)
        {
            List<KLineItem> klines = (from x in orderList
                                     group x by (int)((Utils.ParseDateTime(x.DealTime,Constants.DATEFORMAT_NUMONLY) - Constants.LOWDATE).TotalMinutes / intervalMinutes) into xx
                                     select new KLineItem()
                                     {
                                         IntervalMinutes = intervalMinutes,
                                         Close = xx.Last().BuyRequestPrice,
                                         High = xx.Max(y => y.BuyRequestPrice),
                                         KLineTimeString = Constants.LOWDATE.AddMinutes((int)((xx.Min(y => Utils.ParseDateTime(y.DealTime, Constants.DATEFORMAT_NUMONLY)) - Constants.LOWDATE).TotalMinutes / intervalMinutes) * intervalMinutes).ToString(Constants.DATEFORMAT_NUMONLY),
                                         Low = xx.Min(y => y.BuyRequestPrice),
                                         Open = xx.First().BuyRequestPrice,
                                         Volume = xx.Sum(y => y.DealAmount)
                                     }).ToList();
            List<KLineItem> result = new List<KLineItem>();
            DateTime tempTime = startTime;
            if (latestKLine == null)
                tempTime = Utils.ParseDateTime(klines.Min(x => x.KLineTimeString),Constants.DATEFORMAT_NUMONLY);
            while (tempTime < endTime)
            {
                result.Add(new KLineItem()
                {
                    Close = 0f,
                    High = 0f,
                    IntervalMinutes = intervalMinutes,
                    KLineTimeString = tempTime.ToString(Constants.DATEFORMAT_NUMONLY),
                    Low = 0f,
                    Open = 0f,
                    Volume = 0f
                });
                tempTime = tempTime.AddMinutes(intervalMinutes);
            }
            result = (from x in result
                      join y in klines on x.KLineTimeString equals y.KLineTimeString into yy
                      from tmpKLine in yy.DefaultIfEmpty()
                      select new KLineItem()
                      {
                          Close = tmpKLine != null ? tmpKLine.Close : 0f,
                          High = tmpKLine != null ? tmpKLine.High : 0f,
                          IntervalMinutes = x.IntervalMinutes,
                          KLineTimeString = x.KLineTimeString,
                          Low = tmpKLine != null ? tmpKLine.Low : 0f,
                          Open =  tmpKLine != null ?tmpKLine.Open : 0f,
                          Volume = tmpKLine != null ? tmpKLine.Volume : 0f
                      }).ToList();
            KLineItem tempKline = latestKLine == null? new KLineItem() : latestKLine;
            for (int i = 0; i < result.Count; i ++)
            {
                if (result[i].Volume != 0f)
                {
                    tempKline = result[i];
                    continue;
                }
                result[i].Close = tempKline.Close;
                result[i].High = tempKline.High;
                result[i].IntervalMinutes = tempKline.IntervalMinutes;
                result[i].Low = tempKline.Low;
                result[i].Open = tempKline.Open;
                result[i].Volume = 0f;
                tempKline = result[i];
            }
            return result;
        }
    }
}
