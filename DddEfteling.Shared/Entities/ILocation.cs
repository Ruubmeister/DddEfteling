using Geolocation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.Shared.Entities
{
    public interface ILocation
    {
        public string Name { get; }

        [JsonIgnore]
        public SortedDictionary<double, Guid> DistanceToOthers { get; }

        public Coordinate Coordinates { get; }
        public LocationType LocationType { get;  }
    }
}
