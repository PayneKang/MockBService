using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CommonLib;
using System.Globalization;
using MockJsonServices.Models;
using BitCoinTradeSystem.Models;

namespace MockJsonServices.Controllers
{
    public class KLineController : Controller
    {

        public JsonResult GetLatestKLineTime()
        {
            return Json(DateTime.Now.Date.ToString(Constants.DATEFORMAT_NUMONLY), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServerTimeInterface()
        {
            return Json(DateTime.Now.ToString(Constants.DATEFORMAT_NUMONLY), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderList()
        {
            DateTime startTime = Utils.ParseDateTime(Request["startTime"], Constants.DATEFORMAT_NUMONLY);
            DateTime toTime = Utils.ParseDateTime(Request["toTime"], Constants.DATEFORMAT_NUMONLY);
            string identiryID = Request["identifyID"];
            List<TradeOrder> orders = MockOrders.Orders.OrderBy(x => x.DealTime).Where(x => Utils.ParseDateTime(x.DealTime, Constants.DATEFORMAT_NUMONLY) >= startTime && Utils.ParseDateTime(x.DealTime, Constants.DATEFORMAT_NUMONLY) < toTime).ToList();
            return Json(orders,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLatestKLine()
        {
            DateTime dt = Utils.ParseDateTime(Request["beforeTime"], Constants.DATEFORMAT_NUMONLY);
            return Json(MockOrders.KLines.OrderByDescending(x => x.KLineTimeString).FirstOrDefault(x => Utils.ParseDateTime( x.KLineTimeString, Constants.DATEFORMAT_NUMONLY) <= dt),JsonRequestBehavior.AllowGet);
        }

    }
}
