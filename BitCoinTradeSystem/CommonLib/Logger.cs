using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;

namespace CommonLib
{
    public class Logger
    {
        private Logger() { }
        public static string LogFile { get; private set; }
        public static string LogMsg { get; private set; }
        public static object Lock { get; private set; }
        static Logger()
        {
            LogFile = ConfigurationManager.AppSettings["LogFile"];
            //LogFile = "D:\\log{Date}.txt";
            LogMsg = ConfigurationManager.AppSettings["LogMsg"];
            Lock = new object();
        }
        public static void Log(string msg)
        {
            lock (Lock)
            {
                FileStream fs = File.Open(GetFilePath(), FileMode.Append);
                byte[] buffer = Encoding.UTF8.GetBytes(GetFormattedMessage(msg));
                fs.Write(buffer, 0, buffer.Length);
                fs.Close();
            }
        }
        private static string GetFormattedMessage(string msg)
        {
            string rtnmsg = LogMsg.Replace("{DateTime}", DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]"));
            rtnmsg = rtnmsg.Replace("{Msg}", msg);
            return rtnmsg + "\r\n";
        }
        private static string GetFilePath()
        {
            return LogFile.Replace("{Date}", DateTime.Now.ToString("yyyyMMdd"));
        }
    }
}
