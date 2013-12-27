using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeFuncLib;
using System.Diagnostics;
using System.Web.Script.Serialization;
using BitCoinTradeSystem.Models;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TradeEngineer eng = new TradeEngineer();
            long i = 0;
            while (true)
            {
                i++;
                TradeOrderResponse orders = eng.Calculate(i.ToString().PadLeft(10,' '));

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string jsonoutput = serializer.Serialize(orders);
                Debug.WriteLine(jsonoutput);
            }
        }
    }
}
