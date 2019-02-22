using System;
using System.IO;
using System.Xml.Serialization;

namespace XJPart
{
    public class Utility
    {
        public static string Serialize<T>(T obj)
        {
            XmlSerializer writer = new XmlSerializer(typeof(T));
            using (StringWriter sw = new StringWriter())
            {
                writer.Serialize(sw, obj);
                System.Diagnostics.Debug.WriteLine(sw.GetStringBuilder().ToString());
                return sw.ToString();
            }
        }

        public static T Deserialize<T>(string serializedString)
        {
            StringReader sr = new StringReader(serializedString);
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)(serializer.Deserialize(sr));
        }
    }
}
