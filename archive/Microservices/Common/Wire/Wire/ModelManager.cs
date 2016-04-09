using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace LooksFamiliar.Microservices.Common.Wire
{
    public static class ModelManager
    {
        public static T JsonToModel<T>(string objString)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(objString, settings);
        }

        public static string ModelToJson<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T XmlToModel<T>(string objString)
        {
            var serializer = new XmlSerializer(typeof(T));

            var reader = new StreamReader(objString);
            var obj = (T)serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static string ModelToXml<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof (T));
            var sww = new StringWriter();
            var writer = XmlWriter.Create(sww);
            serializer.Serialize(writer, obj);
            return sww.ToString();;
        }
    }
}