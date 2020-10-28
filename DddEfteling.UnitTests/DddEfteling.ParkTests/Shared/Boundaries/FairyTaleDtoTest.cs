using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class FairyTaleDtoTest
    {

        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            FairyTaleDto fairyTaleDto = new FairyTaleDto(Guid.NewGuid(), "name", new Coordinate(5.23, 51.22), LocationType.FAIRYTALE);

            Assert.Equal("name", fairyTaleDto.Name);
            Assert.Equal(LocationType.FAIRYTALE, fairyTaleDto.LocationType);
            Assert.Equal(5.23, fairyTaleDto.Coordinates.Latitude);
            Assert.Equal(51.22, fairyTaleDto.Coordinates.Longitude);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            FairyTaleDto fairyTaleDto = new FairyTaleDto();
            Assert.Null(fairyTaleDto.Name);
            Assert.Equal(LocationType.FAIRYTALE, fairyTaleDto.LocationType);
            Assert.NotEqual(5.23, fairyTaleDto.Coordinates.Latitude);
            Assert.NotEqual(51.22, fairyTaleDto.Coordinates.Longitude);

            fairyTaleDto.Name = "name";
            fairyTaleDto.Coordinates = new Coordinate(5.23, 51.22);
            fairyTaleDto.Guid = Guid.NewGuid();
            fairyTaleDto.LocationType = LocationType.FAIRYTALE;

            Assert.Equal("name", fairyTaleDto.Name);
            Assert.Equal(LocationType.FAIRYTALE, fairyTaleDto.LocationType);
            Assert.Equal(5.23, fairyTaleDto.Coordinates.Latitude);
            Assert.Equal(51.22, fairyTaleDto.Coordinates.Longitude);
        }
    }
}
