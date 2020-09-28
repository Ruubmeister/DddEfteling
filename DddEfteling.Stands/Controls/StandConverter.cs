using DddEfteling.Stands.Entities;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DddEfteling.Stands.Controls
{
    public class StandConverter: JsonConverter
    {

        public StandConverter()
        {
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(obj["products"].ToString());

            return new Stand(
                obj["name"].ToString(),
                realmControl.FindRealmByName(obj["realm"].ToString()),
                new Coordinate(double.Parse(obj["coordinates"]["lat"].ToString()), double.Parse(obj["coordinates"]["long"].ToString())),
                products
                );
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Stand));
        }
    }

}
