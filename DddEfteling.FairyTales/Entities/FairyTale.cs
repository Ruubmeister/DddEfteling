using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace DddEfteling.FairyTales.Entities
{
    public class FairyTale : Location
    {

        public FairyTale() : base(Guid.NewGuid(), LocationType.FAIRYTALE) { }

        public FairyTale(string name, Coordinate coordinates): base(Guid.NewGuid(), LocationType.FAIRYTALE)
        {
            Name = name;
            Coordinates = coordinates;
        }

        public FairyTale(JObject obj): base(Guid.NewGuid(), LocationType.FAIRYTALE)
        {
            Name = obj["name"].ToString();
            Coordinates = new Coordinate(
                obj["coordinates"]["lat"].ToObject<double>(),
                obj["coordinates"]["long"].ToObject<double>()
                );
        }

        [JsonIgnore]
        public ConcurrentDictionary<Guid, DateTime> VisitorWithTimeDone { get; } = new ();
        

        public FairyTaleDto ToDto()
        {
            return new FairyTaleDto(Guid, Name, Coordinates, LocationType);
        }
    }
}
