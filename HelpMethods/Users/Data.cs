using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Content_Management_System.HelpMethods
{
    public class Data
    {
        public void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) return;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var writer = new StreamWriter(fileName))
                {
                    serializer.Serialize(writer, serializableObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return default;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (var reader = new StreamReader(fileName))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return default;
            }
        }

    }
}
