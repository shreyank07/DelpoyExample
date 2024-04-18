using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProdigyFramework.Model
{
    public static class SerializeObject<T> where T : class
    {
        public static void SerializeData(T data, string fileName)
        {
            XmlSerializer objectSerializer = new XmlSerializer(typeof(T));
            using (StreamWriter writer = new StreamWriter(@fileName))
            {
                objectSerializer.Serialize(writer, data);
            }
        }

        public static bool DeserializeData(string fileName, out T data)
        {
            data = null;
            try
            {
                if (File.Exists(@fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    using (var stream = File.OpenRead(fileName))
                    {
                        var deserialized = (T)(serializer.Deserialize(stream));
                        data = deserialized;
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
