using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace BitCoinTradeFuncLib
{
    public static class UrlReader
    {
        public static Encoding ENCODING = Encoding.UTF8;
        public const int BUFFER_SIZE = 1024;
        private static string ReadUrl(string url)
        {
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResp = (HttpWebResponse)httpReq.GetResponse();
            Stream stream = httpResp.GetResponseStream();

            byte[] buffer = new byte[BUFFER_SIZE];
            int read = stream.Read(buffer, 0, BUFFER_SIZE);
            StringBuilder sb = new StringBuilder();
            while (read > 0)
            {
                sb.Append(ENCODING.GetString(buffer, 0, read));
                read = stream.Read(buffer, 0, BUFFER_SIZE);
            }
            stream.Close();
            return sb.ToString();
        }
        public static T GetJsonResponse<T>(string url)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T req = serializer.Deserialize<T>(UrlReader.ReadUrl(url));
            return req;
        }
    }
}
