using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DddEfteling.Shared.Boundaries
{
    public class RideDto : ILocationDto
    {
        public Guid Guid { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public int MinimumAge { get; set; }
        public double MinimumLength { get; set; }
        public int DurationInSec { get; set; }
        public int MaxPersons { get; set; }
        public LocationType LocationType { get; set; }
        public Coordinate Coordinates { get; set; }

        public RideDto()
        {
            LocationType = LocationType.RIDE;
        }


        [SuppressMessage("csharpsquid", "S107")]
        public RideDto(Guid guid, string name, string status, int minimumAge, double minimumLength, TimeSpan duration, int maxPersons,
            Coordinate coordinate, LocationType locationType)
        {
            this.Guid = guid;
            this.Name = name;
            this.Status = status;
            this.MinimumAge = minimumAge;
            this.MinimumLength = minimumLength;
            this.DurationInSec = (int)duration.TotalSeconds;
            this.MaxPersons = maxPersons;
            this.Coordinates = coordinate;
            this.LocationType = locationType;
        }
    }
}
