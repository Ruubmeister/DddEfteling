using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public interface ILocationDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public LocationType LocationType { get; set; }
    }
}
