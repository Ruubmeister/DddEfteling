using System;
using DddEfteling.Shared.Entities;
using Geolocation;
using Xunit;

namespace DddEfteling.ParkTests.Entities
{
    public class LocationTest
    {
        [Fact]
        public void Location_SetFields_ExpectCorrectValues()
        {
            Coordinate coordinates = new Coordinate(50.0, 5.0);
            LocationImpl location = new LocationImpl()
            {
                Coordinates = coordinates,
                LocationType = LocationType.STAND,
                Name = "Test location"
            };
            
            Assert.Equal("Test location", location.Name);
            Assert.Equal(coordinates, location.Coordinates);
            Assert.Equal(LocationType.STAND, location.LocationType);
            Assert.False(location.Guid == Guid.Empty);
        }
    }
}