using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using BitCoinTradeSystem.Models;

namespace BitCoinTradeFuncLib
{
    public class KLineEngineer
    {
        private const string GET_LATESTKLINETIME_INTERFACE = "http://localhost:50580/KLine/GetLatestKLineTime";
        private const string GET_ORDERLIST_INTERFACE = "http://localhost:50580/Kline/GetOrderList?startTime={0}&toTime={1}&identifyID={2}";
        private const string GET_KLINE_INTERFACE = "http://localhost:50580/KLine/GetKLineList?startTime={0}&toTime={1}&identifyID={2}";
        private const int MINUTES_PER_TIME = 60;
        private DateTime GetTheLatestDateTime()
        {
            return UrlReader.GetJsonResponse<DateTime>(GET_LATESTKLINETIME_INTERFACE);
        }
        private List<TradeOrder> GetTradeOrder(DateTime startTime, DateTime toTime, string orderType, string identifyID)
        {
            string url = string.Format(GET_ORDERLIST_INTERFACE, startTime.ToString(Consts.DATEFORMAT_NUMONLY),toTime.ToString(Consts.DATEFORMAT_NUMONLY),identifyID);
            return UrlReader.GetJsonResponse<List<TradeOrder>>(url);
        }
        private List<KLineItem> GetCurrentKLine(DateTime startTime, DateTime toTime, string identifyID)
        {
            string url = string.Format(GET_KLINE_INTERFACE, startTime.ToString(Consts.DATEFORMAT_NUMONLY), toTime.ToString(Consts.DATEFORMAT_NUMONLY), identifyID);
            return UrlReader.GetJsonResponse<List<KLineItem>>(url);
        }
        public List<KLineItem> Calculate(string identifyID)
        {
            DateTime startTime = GetTheLatestDateTime();
            DateTime toTime = startTime.AddMinutes(MINUTES_PER_TIME);
            List<TradeOrder> orders = GetTradeOrder(startTime, toTime, Consts.BUY_CODE, identifyID);
            List<KLineItem> klines = GetCurrentKLine(startTime, toTime, identifyID);
            Array klinetypes = Enum.GetValues(typeof(KLineType));
            List<KLineItem> result = new List<KLineItem>();
            foreach (int  ktype in klinetypes)
            {
                List<KLineItem> targets = CalculateKLine(orders, ktype);
                result.AddRange(UpdateKLine(klines, targets,ktype));
            }
            return result;
        }
        private List<KLineItem> UpdateKLine(List<KLineItem> source, List<KLineItem> target,int ktype)
        {
            if(target == null || target.Count == 0)
                return source.Where(x=>x.IntervalMinutes == ktype).ToList();
            int interval = ktype;
            List<KLineItem> result = new List<KLineItem>();
            List<KLineItem> tempSource = source.Where(x => x.IntervalMinutes == interval).ToList();
            List<KLineItem> extraTarget = (from x in target
                                           from y in tempSource
                                           where x.KLineTime != y.KLineTime
                                           select x).ToList();
            tempSource.AddRange(extraTarget);
            foreach (KLineItem sourceItem in tempSource)
            {
                KLineItem targetItem = target.FirstOrDefault(x => x.KLineTime == sourceItem.KLineTime);
                if (target == null)
                {
                    result.Add(sourceItem);
                    continue;
                }

                KLineItem tempItem = new KLineItem()
                {
                    Close = targetItem.Close,
                    High = Math.Max(sourceItem.High, targetItem.High),
                    IntervalMinutes = sourceItem.IntervalMinutes,
                    KLineTime = sourceItem.KLineTime,
                    Low = Math.Min(sourceItem.Low, targetItem.Low),
                    Open = sourceItem.Open,
                    Volume = sourceItem.Volume + targetItem.Volume
                };
                result.Add(tempItem);
            }
            return result;
        }
        private List<KLineItem> CalculateKLine(List<TradeOrder> orderList,int ktype)
        {
            var lowDate = DateTime.Parse(Consts.LOWDATE_STR);
            int interval = ktype;
            List<KLineItem> group = (from x in orderList
                         group x by (x.DealTime - lowDate).TotalMinutes / interval into xx
                         select new KLineItem()
                         {
                             IntervalMinutes = interval,
                             Close = xx.Last().BuyRequestPrice,
                             High = xx.Max(y => y.BuyRequestPrice),
                             KLineTime = lowDate.AddMinutes(((xx.Min(y => y.DealTime) - lowDate).TotalMinutes / interval) * interval),
                             Low = xx.Min(y => y.BuyRequestPrice),
                             Open = xx.First().BuyRequestPrice,
                             Volume = xx.Sum(y => y.DealAmount)
                         }).ToList();            
            return group;
        }
    }
}
