using System;
using DddEfteling.Shared.Entities;
using Geolocation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DddEfteling.Shared.Controls
{
    public class LocationConverter<T> : JsonConverter where T: Location, new()
    {
        private readonly Func<JObject, T> instantiator;
         
        public LocationConverter(Func<JObject, T> instantiator)
        {
            this.instantiator = instantiator;
        }

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(T));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException("Not implemented");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            return instantiator(JObject.Load(reader));
        }

        public override bool CanWrite
        {
            get { return false; }
        }
    }
}