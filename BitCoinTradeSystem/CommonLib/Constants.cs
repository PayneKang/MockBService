using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Constants
    {
        private Constants() { }
        public const string BUY_CODE = "buy";
        public const string SELL_CODE = "sell";
        public const string DATEFORMAT_NUMONLY = "yyyyMMddHHmmss";
        public readonly static DateTime LOWDATE = DateTime.Parse("1900-01-01");
    }
}
