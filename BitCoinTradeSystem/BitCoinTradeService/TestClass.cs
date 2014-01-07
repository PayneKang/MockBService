using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitCoinTradeFuncLib;
using CommonLib;
using BitCoinTradeSystem.Models;

namespace BitCoinTradeService
{
    class TestClass
    {
        public void Start()
        {
            TradeEngineer eng = new TradeEngineer();
            RandomProvider ran = new RandomProvider();
            Logger.Log("start trade order calculate engineer");
            while (true)
            {
                try
                {
                    string key = ran.GetRandomString(10);
                    Logger.Log(string.Format(" -- Start to calculate order, Key {0}", key));
                    TradeOrderResponse orders = eng.Calculate(key);
                    Logger.Log(string.Format("  -- Start to send data back, Key {0}", orders.IdentifyID));
                    eng.SendOrderToResponse(orders);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex.Message);
                }
                System.Threading.Thread.Sleep(TradeEngineer.ORDER_INTERVAL);
            }
        }
    }
}
