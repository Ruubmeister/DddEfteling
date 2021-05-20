using System;
using System.Buffers.Text;
using DddEfteling.Shared.Entities;
using Geolocation;

namespace DddEfteling.ParkTests.Entities
{
    public class LocationImpl: Location
    {
        public LocationImpl(Coordinate coordinate) : base(Guid.NewGuid(), LocationType.RIDE)
        {
            this.Coordinates = coordinate;
        }
    }
}