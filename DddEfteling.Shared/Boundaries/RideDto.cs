using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
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

        public int VisitorsInLine { get; set; }
        public int VisitorsInRide { get; set; }
        public string EndTime { get; set; }

        public Dictionary<string, string> EmployeesToSkill { get; set; } = new Dictionary<string, string>();


        public RideDto()
        {
            LocationType = LocationType.RIDE;
        }


        [SuppressMessage("csharpsquid", "S107")]
        public RideDto(Guid guid, string name, string status, int minimumAge, double minimumLength, TimeSpan duration, int maxPersons,
            Coordinate coordinate, LocationType locationType)
        {
            Guid = guid;
            Name = name;
            Status = status;
            MinimumAge = minimumAge;
            MinimumLength = minimumLength;
            DurationInSec = (int)duration.TotalSeconds;
            MaxPersons = maxPersons;
            Coordinates = coordinate;
            LocationType = locationType;
        }
    }
}
