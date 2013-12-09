using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ML.Xml
{
    public class XmlSerialization
    {
        private static byte[] XmlSerializeInternal(object obj, Encoding encoding)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("object can not be null");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding can not be null");
            }

            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            byte[] result;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlSerializer.Serialize(xmlTextWriter, obj);
                    xmlTextWriter.Close();
                }
                result = memoryStream.ToArray();
            }
            return result;
        }

        public static string XmlSerialize(object obj, Encoding encoding)
        {
            byte[] bytes = XmlSerializeInternal(obj, encoding);
            return encoding.GetString(bytes);
        }

        public static void XmlSerializeToFile(object obj, string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path can not be null");
            }
            byte[] bytes = XmlSerializeInternal(obj, encoding);
            File.WriteAllBytes(path, bytes);
        }

        public static T XmlDeserialize<T>(string strStream, Encoding encoding)
        {
            if (string.IsNullOrEmpty(strStream))
            {
                throw new ArgumentNullException("strStream can not be null");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding can not be null");
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            T result;
            using (MemoryStream memoryStream = new MemoryStream(encoding.GetBytes(strStream)))
            {
                using (StreamReader streamReader = new StreamReader(memoryStream, encoding))
                {
                    result = (T)(xmlSerializer.Deserialize(streamReader));
                }
            }
            return result;
        }

        public static T XmlDeserializeFromFile<T>(string path, Encoding encoding)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path can not be null");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding can not be null");
            }
            string s = File.ReadAllText(path, encoding);
            return XmlDeserialize<T>(s, encoding);
        }
    }
}
