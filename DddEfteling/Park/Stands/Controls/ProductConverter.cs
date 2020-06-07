using DddEfteling.Park.Stands.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DddEfteling.Park.Stands.Controls
{
    public class ProductConverter: JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Product(
                obj["name"].ToString(),
                float.Parse(obj["price"].ToString()),
                (ProductType) Enum.Parse(typeof(ProductType), obj["type"].ToString())
                );
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Product));
        }
    }

}
