using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeFuncLib;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TradeEngineer eng = new TradeEngineer();
            string result = eng.ReadUrl(TradeEngineer.GET_HIGHESTBUY_INTERFACE);
            string result2 = eng.ReadUrl(TradeEngineer.GET_LOWESTSELL_INTERFACE);
            List<BitCoinTradeSystem.Models.TradeOrder> orders = eng.Calculate();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string jsonoutput = serializer.Serialize(orders);
            Debug.WriteLine(jsonoutput);
        }
    }
}
