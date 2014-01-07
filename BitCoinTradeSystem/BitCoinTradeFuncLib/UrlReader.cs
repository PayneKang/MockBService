using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace BitCoinTradeFuncLib
{
    public enum SendType{
        Get,
        Post
    }
    public class ParameterValue{
        public string Name{get;set;}
        public string Value{get;set;}
        public string ToParameterString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
    }
    public static class UrlReader
    {
        public static Encoding ENCODING = Encoding.UTF8;
        public const int BUFFER_SIZE = 1024;
        private static string ReadUrl(string url, SendType sendType = SendType.Get, string postData = "" )
        {
            HttpWebRequest httpReq = null;
            if (sendType == SendType.Post)
            {
                Uri uri = new Uri(url);
                httpReq = (HttpWebRequest)WebRequest.Create(uri);
                httpReq.Method = "POST";
                httpReq.ContentType = "application/x-www-form-urlencoded";
                httpReq.ContentLength = postData.Length;
                using (Stream writeStream = httpReq.GetRequestStream())
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] bytes = encoding.GetBytes(postData);
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                Uri uri = new Uri(url + "?" + postData);
                httpReq = (HttpWebRequest)WebRequest.Create(uri);
                httpReq.Method = "GET";
            }
            string result = string.Empty;
            using (HttpWebResponse response = (HttpWebResponse)httpReq.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                }
            }
            return result;
        }
        public static T GetJsonResponse<T>(string url, SendType sendType = SendType.Get, ParameterValue[] parameters = null )
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            T req = serializer.Deserialize<T>(UrlReader.ReadUrl(url,sendType,BuilResponseString(parameters)));
            return req;
        }
        private static string BuilResponseString(ParameterValue[] parameters)
        {
            if(parameters == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (ParameterValue parameter in parameters)
            {
                if (isFirst)
                {
                    sb.Append(parameter.ToParameterString());
                    isFirst = false;
                    continue;
                }
                sb.Append(string.Format("&{0}", parameter.ToParameterString()));
            }
            return sb.ToString();
        }
    }
}
