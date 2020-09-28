using DddEfteling.FairyTales.Entities;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace DddEfteling.FairyTales.Controls
{
    public class FairyTaleConverter : JsonConverter
    {

        public FairyTaleConverter()
        {
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
