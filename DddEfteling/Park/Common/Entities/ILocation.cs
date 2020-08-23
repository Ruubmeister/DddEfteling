using Geolocation;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.Park.Common.Entities
{
    public interface ILocation
    {
        public string Name { get; }

        [JsonIgnore]
        public SortedDictionary<double, string> DistanceToOthers { get; }

        public Coordinate Coordinates { get; }
        public LocationType LocationType { get;  }
    }
}
