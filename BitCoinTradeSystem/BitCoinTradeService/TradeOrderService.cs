using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using CommonLib;
using BitCoinTradeSystem.Models;
using BitCoinTradeFuncLib;
using System.Threading;

namespace BitCoinTradeService
{
    public partial class TradeOrderService : ServiceBase
    {
        public TradeOrderService()
        {
            InitializeComponent();
        }

        private Thread _thread;

        protected override void OnStart(string[] args)
        {
            _thread = new Thread(StartEngineer);
            _thread.Start();
            Logger.Log("Start engineer thread success");
        }

        protected override void OnStop()
        {
            _thread.Abort();
            Logger.Log("Stop engineer thread");
        }

        private void StartEngineer()
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
