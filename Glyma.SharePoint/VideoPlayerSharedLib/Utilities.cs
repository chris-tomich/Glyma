using System;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace VideoPlayerSharedLib
{
    public class Utilities
    {
        public static string Serialize<T>(T data)
        {
            using (var memoryStream = new MemoryStream())
            {
                var serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, data);
                memoryStream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(memoryStream);
                string content = reader.ReadToEnd();
                return content;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            using (var stream = new MemoryStream(Encoding.Unicode.GetBytes(xml)))
            {
                var serializer = new DataContractSerializer(typeof(T));
                T theObject = (T)serializer.ReadObject(stream);
                return theObject;
            }
        }

        public static void SendMessage<T>(JSMessageSender sender, T msgObj)
        {
            string serializedObject = Serialize<T>(msgObj);
            sender.SendMessage(serializedObject);
        }

        public static List<EventArg> ConvertParamsToEventArgs(List<Param> commandParams)
        {
            List<EventArg> result = new List<EventArg>();
            if (commandParams != null)
            {
                foreach (Param commandParam in commandParams) {
                    result.Add(new EventArg() { Name = commandParam.Name, Value = commandParam.Value });
                }
            }
            return result;
        }
    }
}
