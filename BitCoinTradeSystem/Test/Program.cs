using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeFuncLib;
using System.Diagnostics;
using System.Web.Script.Serialization;
using BitCoinTradeSystem.Models;
using CommonLib;
using System.Reflection;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            TradeEngineer eng = new TradeEngineer();
            RandomProvider ran = new RandomProvider();
            while (true)
            {
                string key = ran.GetRandomString(10);
                Logger.Log("start engineer");
                TradeOrderResponse orders = eng.Calculate(key);
                eng.SendOrderToResponse(orders);
                System.Threading.Thread.Sleep(TradeEngineer.ORDER_INTERVAL);
            }
        }
    }
}
