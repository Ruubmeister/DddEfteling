using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Stands.Entities;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Stands.Controls
{
    public class StandConverter: JsonConverter
    {

        private readonly IRealmControl realmControl;

        public StandConverter(IRealmControl realmControl)
        {
            this.realmControl = realmControl;
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
