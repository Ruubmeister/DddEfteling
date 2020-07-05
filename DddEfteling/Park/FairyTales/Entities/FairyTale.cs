using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using Geolocation;
using System;
using System.Collections.Immutable;

namespace DddEfteling.Park.FairyTales.Entities
{
    public class FairyTale : ILocation
    {
        public FairyTale(String name, Realm realm, Coordinate coordinates)
        {
            this.Name = name;
            this.Realm = realm;
            this.Coordinates = coordinates;
        }
        public ImmutableSortedDictionary<string, double> DistanceToOthers { get; set; }

        public string Name { get; }

        public Realm Realm { get;  }

        public Coordinate Coordinates { get; }

        public double GetDistanceTo(FairyTale tale)
        {
            return GeoCalculator.GetDistance(this.Coordinates, tale.Coordinates, 2, DistanceUnit.Meters);
        }
    }
}
