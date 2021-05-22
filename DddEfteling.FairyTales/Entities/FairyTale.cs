using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.FairyTales.Entities
{
    public class FairyTale : Location
    {

        public FairyTale() : base(Guid.NewGuid(), LocationType.FAIRYTALE) { }

        public FairyTale(String name, Coordinate coordinates): base(Guid.NewGuid(), LocationType.FAIRYTALE)
        {
            this.Name = name;
            this.Coordinates = coordinates;
        }

        [JsonIgnore]
        public ConcurrentDictionary<Guid, DateTime> VisitorWithTimeDone { get; } = new ConcurrentDictionary<Guid, DateTime>();
        

        public FairyTaleDto ToDto()
        {
            return new FairyTaleDto(this.Guid, this.Name, this.Coordinates, this.LocationType);
        }
    }
}
