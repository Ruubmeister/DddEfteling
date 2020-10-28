using DddEfteling.Shared.Boundaries;
using Geolocation;
using System;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class VisitorDtoTest
    {
        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            DateTime now = DateTime.Now;
            VisitorDto visitorDto = new VisitorDto(Guid.NewGuid(), now, 1.88, new Coordinate(5.23, 51.22));

            Assert.Equal(now, visitorDto.DateOfBirth);
            Assert.Equal(1.88, visitorDto.Length);
            Assert.Equal(5.23, visitorDto.CurrentLocation.Latitude);
            Assert.Equal(51.22, visitorDto.CurrentLocation.Longitude);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            DateTime now = DateTime.Now;
            VisitorDto visitorDto = new VisitorDto();

            visitorDto.DateOfBirth = now;
            visitorDto.CurrentLocation = new Coordinate(5.23, 51.22);
            visitorDto.Guid = Guid.NewGuid();
            visitorDto.Length = 1.76;

            Assert.Equal(now, visitorDto.DateOfBirth);
            Assert.Equal(1.76, visitorDto.Length);
            Assert.Equal(5.23, visitorDto.CurrentLocation.Latitude);
            Assert.Equal(51.22, visitorDto.CurrentLocation.Longitude);
        }
    }
}
