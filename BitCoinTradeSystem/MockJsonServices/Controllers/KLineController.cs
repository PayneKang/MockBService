using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MockJsonServices.Controllers
{
    public class KLineController : Controller
    {

        public JsonResult GetLatestKLineTime()
        {
            return Json(DateTime.Now.Date, JsonRequestBehavior.AllowGet);
        }

    }
}
