using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitCoinTradeSystem.Models
{
    /// <summary>
    /// K线数据模型
    /// </summary>
    public class KLineItem
    {
        /// <summary>
        /// K线间隔时间，以分钟为单位
        /// </summary>
        public int IntervalMinutes {get;set;}
        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open{get;set;}
        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }
        /// <summary>
        /// 最低价
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }
        /// <summary>
        /// 交易量
        /// </summary>
        public double Volume { get; set; }
        /// <summary>
        /// K线时间 yyyyMMddHHmmss
        /// </summary>
        public string KLineTimeString { get; set; }
    }
}
