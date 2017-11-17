using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bitfinex.Net.Converters
{
    public class BitfinexResultConverter: JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = Activator.CreateInstance(objectType);
            var arr = JArray.Load(reader);
            foreach (var property in objectType.GetProperties())
            {
                var attribute =
                    (BitfinexPropertyAttribute) property.GetCustomAttribute(typeof(BitfinexPropertyAttribute));
                if (attribute == null)
                    continue;

                if (attribute.Index >= arr.Count)
                    continue;

                object value;
                var converterAttribute = (JsonConverterAttribute) property.GetCustomAttribute(typeof(JsonConverterAttribute));
                if (converterAttribute != null)
                    value = arr[attribute.Index].ToObject(property.PropertyType, new JsonSerializer() { Converters = { (JsonConverter)Activator.CreateInstance(converterAttribute.ConverterType) } });
                else
                    value = arr[attribute.Index];                

                if (property.PropertyType.IsAssignableFrom(value.GetType()))
                    property.SetValue(result, value);
                else
                    property.SetValue(result, value == null ? null : Convert.ChangeType(value, property.PropertyType));
            }
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }
    }
}
