using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using SilverlightMappingToolBasic.UI.SuperGraph.ViewModel;

namespace SilverlightMappingToolBasic.UI.Extensions.Json 
{ 
    public static class JsonSerializerHelper 
    {
        /// <summary> 
        /// Adds an extension method to a string 
        /// </summary> 
        /// <typeparam name="TObj">The expected type of Object</typeparam> 
        /// <param name="json">Json string data</param> 
        /// <returns>The deserialized object graph</returns> 
        public static TObj JsonDeserialize<TObj>(this string json)
        {
            using (var mstream = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(TObj));
                return (TObj)serializer.ReadObject(mstream);
            }
        }

        /// <summary> 
        /// Serialize the object to Json string 
        /// </summary> 
        /// <param name="obj">Object to serialize</param> 
        /// <returns>Serialized string</returns> 
        public static string JsonSerialize(this object obj)
        {
            using (var mstream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(obj.GetType());
                serializer.WriteObject(mstream, obj);
                mstream.Position = 0;
                using (var reader = new StreamReader(mstream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static KeyValuePair<string, JsonValue> CreateProperty(string name, dynamic value)
        {
            var dic = value as Dictionary<string, object>;
            if (dic != null)
            {
                return new KeyValuePair<string, JsonValue>(name, dic.ToJsonArray());
            }
            return new KeyValuePair<string, JsonValue>(name, new JsonPrimitive(value));
        }


        public static string ToJson(this Dictionary<string, object> node)
        {
            return ToJsonObject(node).ToString();
        }

        private static JsonArray ToJsonArray(this Dictionary<string, object> node)
        {
            var result = from item in node
                         select new JsonObject(CreateProperty(item.Key, item.Value));
            return new JsonArray(result);
        }

        private static JsonObject ToJsonObject(this Dictionary<string, object> node)
        {
            var result = new List<KeyValuePair<string, JsonValue>>();
            foreach (var item in node)
            {
                var dic = item.Value as Dictionary<string, object>;
                if (dic != null)
                {
                    result.Add(new KeyValuePair<string, JsonValue>(item.Key, dic.ToJsonObject()));
                }
                else
                {
                    result.Add(new KeyValuePair<string, JsonValue>(item.Key, new JsonPrimitive(item.Value.ToString())));
                }
                
            }
            return new JsonObject(result);
        }

        public static string ToJson(this IEnumerable<Guid> list)
        {
            if (list.Any())
            {
                return list.JsonSerialize();
            }
            return null;
        }

        public static IEnumerable<Guid> ToGuidList(this string json)
        {
            var output = new List<Guid>();
            if (!string.IsNullOrEmpty(json))
            {
                var converted = json.JsonDeserialize<IEnumerable<Guid>>();
                output.AddRange(converted);
            }
            return output;
        }
    } 
}