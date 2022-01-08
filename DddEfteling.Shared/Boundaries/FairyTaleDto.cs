using DddEfteling.Shared.Entities;
using Geolocation;
using System;

namespace DddEfteling.Shared.Boundaries
{
    public class FairyTaleDto : ILocationDto
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public Coordinate Coordinates { get; set; }

        public LocationType LocationType { get; set; }

        public FairyTaleDto()
        {
            LocationType = LocationType.FAIRYTALE;
        }

        public FairyTaleDto(Guid guid, string name, Coordinate coordinate, LocationType locationType)
        {
            Guid = guid;
            Name = name;
            Coordinates = coordinate;
            LocationType = locationType;
        }
    }
}
