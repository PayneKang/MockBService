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
            TestKLine();
            //TestTrade();
        }

        static void TestKLine()
        {
            KLineEngineer eng = new KLineEngineer();
            RandomProvider ran = new RandomProvider();
            while (true)
            {
                string key = ran.GetRandomString(10);
                Logger.Log("start kline engineer");
                KLineResponse klines = eng.Calculate(key);
                eng.SendKLines(klines);
                System.Threading.Thread.Sleep(KLineEngineer.KLINE_INTERVAL);
            }
        }

        static void TestTrade()
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
