using DddEfteling.Shared.Boundaries;
using Geolocation;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class StandDtoTest
    {
        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            StandDto standDto = new StandDto("name", new Coordinate(5.23, 51.22), new List<string>() { { "meal 1" }, { "meal 2" } }, new List<string>() { { "drink 1" }, { "drink 2" } });

            Assert.Equal("name", standDto.Name);
            Assert.Equal(5.23, standDto.Coordinates.Latitude);
            Assert.Equal(51.22, standDto.Coordinates.Longitude);
            Assert.Contains<string>("meal 1", standDto.Meals);
            Assert.Contains<string>("meal 2", standDto.Meals);
            Assert.Contains<string>("drink 1", standDto.Drinks);
            Assert.Contains<string>("drink 2", standDto.Drinks);

            Assert.Equal(2, standDto.Meals.Count);
            Assert.Equal(2, standDto.Drinks.Count);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            StandDto standDto = new StandDto();

            standDto.Name = "name";
            standDto.Coordinates = new Coordinate(5.23, 51.22);
            standDto.Guid = Guid.NewGuid();
            standDto.Meals = new List<string>() { { "meal 1" }, { "meal 2" } };
            standDto.Drinks = new List<string>() { { "drink 1" }, { "drink 2" } };

            Assert.Equal("name", standDto.Name);
            Assert.Equal(5.23, standDto.Coordinates.Latitude);
            Assert.Equal(51.22, standDto.Coordinates.Longitude);
            Assert.Contains<string>("meal 1", standDto.Meals);
            Assert.Contains<string>("meal 2", standDto.Meals);
            Assert.Contains<string>("drink 1", standDto.Drinks);
            Assert.Contains<string>("drink 2", standDto.Drinks);
        }
    }
}
