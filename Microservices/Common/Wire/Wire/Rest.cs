using System;
using System.IO;
using System.Net;
using System.Text;

namespace LooksFamiliar.Microservices.Common.Wire
{
    public static class Rest
    {
        public static string Get(Uri url)
        {
            var webrequest = WebRequest.Create(url) as HttpWebRequest;
            if (webrequest == null) return null;
            webrequest.Method = WebRequestMethods.Http.Get;
            webrequest.ContentType = "application/json";
            var webresponse = (HttpWebResponse)webrequest.GetResponse();
            var enc = Encoding.GetEncoding("utf-8");
            var responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            var json = responseStream.ReadToEnd();
            webresponse.Close();
            return json;
        }

        public static string Post(Uri url, string payload)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = WebRequestMethods.Http.Post;
            request.Accept = "Accept=application/json";
            var encoding = new ASCIIEncoding();
            var arr = encoding.GetBytes(payload);

            request.ContentType = "application/json";
            request.ContentLength = arr.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd().Trim();
        }

        public static string Put(Uri url, string payload)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = WebRequestMethods.Http.Put;
            request.Accept = "Accept=application/json";
            var encoding = new ASCIIEncoding();
            var arr = encoding.GetBytes(payload);

            request.ContentType = "application/json";
            request.ContentLength = arr.Length;

            var dataStream = request.GetRequestStream();
            dataStream.Write(arr, 0, arr.Length);
            dataStream.Close();

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd().Trim();
        }

        public static string Delete(Uri url)
        {
            var request = WebRequest.Create(url) as HttpWebRequest;
            if (request == null) return null;
            request.Method = "DELETE";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = 0;
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            return reader.ReadToEnd().Trim();
        }
    }
}

