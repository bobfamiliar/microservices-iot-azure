using System;
using System.IO;
using System.Net;
using System.Text;
using LooksFamiliar.Microservices.Biometrics.Models;
using LooksFamiliar.Microservices.Config.Models;
using Newtonsoft.Json;
using LooksFamiliar.Microservices.Config.Public.SDK;

namespace BiometricsDashboard
{  
    public class BiometricReadingsFactory
    {
        public BiometricReadings GetBiometrics(string url)
        {
            BiometricReadings biometrics = null;

            try
            {
                var json = CallRestAPI(url);
                biometrics = Deserialize<BiometricReadings>(json);
            }
            catch (Exception err)
            {
                biometrics = new BiometricReadings();
            }

            return biometrics;
        }

        private static T Deserialize<T>(string json)
        {
            return (T)JsonConvert.DeserializeObject<T>(json);
        }

        private string CallRestAPI(string url)
        {
            HttpWebResponse webresponse;
            StreamReader responseStream;

            Uri requestUri = new Uri(url);
            HttpWebRequest webrequest = WebRequest.Create(requestUri) as HttpWebRequest;

            webrequest.Method = "GET";
            webrequest.ContentType = "application/json";
            var response = webrequest.GetResponse();
            webresponse = (HttpWebResponse)response;

            Encoding enc = Encoding.GetEncoding("utf-8");
            responseStream = new StreamReader(webresponse.GetResponseStream(), enc);

            string json = responseStream.ReadToEnd();

            webresponse.Close();

            return json;
        }
    }
}