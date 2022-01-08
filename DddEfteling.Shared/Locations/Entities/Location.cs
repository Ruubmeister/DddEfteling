using Geolocation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.Shared.Entities
{
    public abstract class Location
    {
        protected Location(
            Guid guid, 
            LocationType type)
        {
            Guid = guid;
            LocationType = type;
        }

        public Guid Guid { get; set; }
        
        public string Name { get; set; }
        
        public Coordinate Coordinates { get; set; }
        public LocationType LocationType { get; set;  }

        [JsonIgnore]
        public SortedDictionary<double, Guid> DistanceToOthers { get; set; } = new();

        public void AddDistanceToOthers(double distance, Guid rideGuid)
        {
            DistanceToOthers.Add(distance, rideGuid);
        }
        
        public double GetDistanceTo(Location location)
        {
            return GeoCalculator.GetDistance(Coordinates, location.Coordinates, 4, DistanceUnit.Meters);
        }
    }
}
