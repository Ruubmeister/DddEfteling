using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class RideDtoTest
    {
        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            RideDto rideDto = new RideDto(Guid.NewGuid(), "name", "status", 13, 1.64, TimeSpan.FromMinutes(11), 55, new Coordinate(5.23, 51.22), LocationType.RIDE);

            Assert.Equal("name", rideDto.Name);
            Assert.Equal(LocationType.RIDE, rideDto.LocationType);
            Assert.Equal(5.23, rideDto.Coordinates.Latitude);
            Assert.Equal(51.22, rideDto.Coordinates.Longitude);
            Assert.Equal("status", rideDto.Status);
            Assert.Equal(13, rideDto.MinimumAge);
            Assert.Equal(1.64, rideDto.MinimumLength);
            Assert.Equal(660, rideDto.DurationInSec);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            RideDto rideDto = new RideDto();

            rideDto.Name = "name";
            rideDto.Coordinates = new Coordinate(5.23, 51.22);
            rideDto.Guid = Guid.NewGuid();
            rideDto.LocationType = LocationType.RIDE;
            rideDto.DurationInSec = 660;
            rideDto.MinimumLength = 1.64;
            rideDto.MinimumAge = 13;
            rideDto.Status = "status";

            Assert.Equal("name", rideDto.Name);
            Assert.Equal(LocationType.RIDE, rideDto.LocationType);
            Assert.Equal(5.23, rideDto.Coordinates.Latitude);
            Assert.Equal(51.22, rideDto.Coordinates.Longitude);
            Assert.Equal(660, rideDto.DurationInSec);
            Assert.Equal(13, rideDto.MinimumAge);
            Assert.Equal(1.64, rideDto.MinimumLength);
            Assert.Equal("status", rideDto.Status);
        }
    }
}
