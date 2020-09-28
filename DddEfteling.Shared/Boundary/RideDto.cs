using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public class RideDto : ILocationDto
    {
        public Guid Guid { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public int MinimumAge { get; set; }
        public double MinimumLength { get; set; }
        public TimeSpan Duration { get; set; }
        public int MaxPersons { get; set; }
        public LocationType LocationType { get; set; }
        public Coordinate Coordinates { get; set; }

        public RideDto(string name, string status, int minimumAge, double minimumLength, TimeSpan duration, int maxPersons,
            Coordinate coordinate, LocationType locationType)
        {
            this.Name = name;
            this.Status = status;
            this.MinimumAge = minimumAge;
            this.MinimumLength = minimumLength;
            this.Duration = duration;
            this.MaxPersons = maxPersons;
            this.Coordinates = coordinate;
            this.LocationType = locationType;
        }
    }
}
