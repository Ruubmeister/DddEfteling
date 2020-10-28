using DddEfteling.Shared.Entities;
using Geolocation;
using System;

namespace DddEfteling.Shared.Boundaries
{
    public interface ILocationDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public LocationType LocationType { get; set; }
    }
}
