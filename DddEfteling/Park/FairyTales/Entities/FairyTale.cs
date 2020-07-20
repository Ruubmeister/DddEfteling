using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DddEfteling.Park.FairyTales.Entities
{
    public class FairyTale : ILocation
    {

        public FairyTale() { }

        public FairyTale(String name, Realm realm, Coordinate coordinates)
        {
            this.Name = name;
            this.Realm = realm;
            this.Coordinates = coordinates;
            LocationType = LocationType.FAIRYTALE;
        }

        public Dictionary<Guid, DateTime> VisitorWithTimeDone { get; } = new Dictionary<Guid, DateTime>();

        public LocationType LocationType { get; }

        public ImmutableSortedDictionary<string, double> DistanceToOthers { get; set; }

        public string Name { get; }

        public Realm Realm { get;  }

        public Coordinate Coordinates { get; }

        public double GetDistanceTo(FairyTale tale)
        {
            return GeoCalculator.GetDistance(this.Coordinates, tale.Coordinates, 2, DistanceUnit.Meters);
        }

        public void AddVisitor(Visitor visitor, DateTime timeDone)
        {
            this.VisitorWithTimeDone.Add(visitor.Guid, timeDone);
        }

        public List<Guid> GetVisitorsDone()
        {
            List<Guid> result = new List<Guid>();
            DateTime now = DateTime.Now;
            foreach ( KeyValuePair<Guid, DateTime> keyValuePair in this.VisitorWithTimeDone.AsEnumerable())
            {
                if(keyValuePair.Value < now)
                {
                    result.Add(keyValuePair.Key);
                    this.VisitorWithTimeDone.Remove(keyValuePair.Key);
                }
            }
            return result;
            
        }
    }
}
