using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BitCoinTradeSystem.Models;

namespace MockJsonServices.Controllers
{
    public class TradeDataController : Controller
    {
        //
        // GET: /TradeData/

        public JsonResult GetHighestBuyRequest()
        {
            return Json(TradeRequestItem.TestBuyList.OrderByDescending(x=>x.Price).FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLowestSellRequest()
        {
            return Json(TradeRequestItem.TestSellList.OrderBy(x=>x.Price).FirstOrDefault(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult TradeOrderCallbackHandles()
        {
            string jsonData = Request.Form["JsonStr"];
            TradeOrderCallback callback = new TradeOrderCallback()
            {
                success = true,
                errormessage = "test message",
                 IdentifyID = Request["IdentifyID"]
            };
            return Json(callback, JsonRequestBehavior.AllowGet);
        }

    }
}
