using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class Utils
    {
        const string YEAR = "yyyy";
        const string MONTH = "MM";
        const string DAY = "dd";
        const string HOUR = "HH";
        const string MINUTE = "mm";
        const string SECOND = "ss";
        public static DateTime ParseDateTime(string dateTimeStr, string format)
        {
            int year = int.Parse(dateTimeStr.Substring(format.IndexOf(YEAR), YEAR.Length));
            int month = int.Parse(dateTimeStr.Substring(format.IndexOf(MONTH), MONTH.Length));
            int day = int.Parse(dateTimeStr.Substring(format.IndexOf(DAY), DAY.Length));
            int hour = int.Parse(dateTimeStr.Substring(format.IndexOf(HOUR), HOUR.Length));
            int minute = int.Parse(dateTimeStr.Substring(format.IndexOf(MINUTE), MINUTE.Length));
            int second = int.Parse(dateTimeStr.Substring(format.IndexOf(SECOND), SECOND.Length));
            return new DateTime(year, month, day, hour, minute, second);
        }
    }
}
