using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DddEfteling.Park.FairyTales.Controls
{
    public class FairyTaleConverter : JsonConverter
    {
        private readonly IRealmControl realmControl;

        public FairyTaleConverter(IRealmControl realmControl)
        {
            this.realmControl = realmControl;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(FairyTale));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented yet");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            return new FairyTale(obj["name"].ToString(),
                realmControl.FindRealmByName(obj["realm"].ToString()),
                new Coordinate(
                    obj["coordinates"]["lat"].ToObject<double>(),
                    obj["coordinates"]["long"].ToObject<double>()
                    )
                );
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }

}
