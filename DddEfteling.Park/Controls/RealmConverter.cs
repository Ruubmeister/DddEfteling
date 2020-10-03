using DddEfteling.Park.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DddEfteling.Park.Controls
{
    public class RealmConverter: JsonConverter
    {

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Realm(obj["name"].ToString());
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Realm));
        }
    }

}
