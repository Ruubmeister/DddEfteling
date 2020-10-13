using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DddEfteling.FairyTales.Entities
{
    public class FairyTale : ILocation
    {

        public FairyTale() { }

        public FairyTale(String name, Coordinate coordinates)
        {
            this.Name = name;
            this.Coordinates = coordinates;
            LocationType = LocationType.FAIRYTALE;
        }

        public Guid Guid { get; } = Guid.NewGuid();

        [JsonIgnore]
        public ConcurrentDictionary<Guid, DateTime> VisitorWithTimeDone { get; } = new ConcurrentDictionary<Guid, DateTime>();

        public LocationType LocationType { get; }

        [JsonIgnore]
        public SortedDictionary<double, Guid> DistanceToOthers { get; } = new SortedDictionary<double, Guid>();

        public string Name { get; }

        public Coordinate Coordinates { get; }

        public double GetDistanceTo(FairyTale tale)
        {
            return GeoCalculator.GetDistance(this.Coordinates, tale.Coordinates, 2, DistanceUnit.Meters);
        }

        public void AddDistanceToOthers(double distance, Guid taleGuid)
        {
            this.DistanceToOthers.Add(distance, taleGuid);
        }

        public FairyTaleDto ToDto()
        {
            return new FairyTaleDto(this.Guid, this.Name, this.Coordinates, this.LocationType);
        }
    }
}
