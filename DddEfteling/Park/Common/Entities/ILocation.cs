﻿using Geolocation;
using System.Collections.Immutable;

namespace DddEfteling.Park.Common.Entities
{
    public interface ILocation
    {
        public string Name { get; }
        public ImmutableSortedDictionary<string, double> DistanceToOthers { get; set; }

        public Coordinate Coordinates { get; }
        public LocationType LocationType { get;  }
    }
}
