using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public ConcurrentDictionary<Guid, DateTime> VisitorWithTimeDone { get; } = new ConcurrentDictionary<Guid, DateTime>();

        public LocationType LocationType { get; }

        [JsonIgnore]
        public SortedDictionary<double, string> DistanceToOthers { get; } = new SortedDictionary<double, string>();

        public string Name { get; }

        public Realm Realm { get;  }

        public Coordinate Coordinates { get; }

        public double GetDistanceTo(FairyTale tale)
        {
            return GeoCalculator.GetDistance(this.Coordinates, tale.Coordinates, 2, DistanceUnit.Meters);
        }

        public void AddVisitor(Visitor visitor, DateTime timeDone)
        {
            this.VisitorWithTimeDone.TryAdd(visitor.Guid, timeDone);
        }

        public void AddDistanceToOthers(double distance, String taleName)
        {
            this.DistanceToOthers.Add(distance, taleName);
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
                    this.VisitorWithTimeDone.TryRemove(keyValuePair.Key, out DateTime dateTime);
                }
            }
            return result;
            
        }
    }
}
