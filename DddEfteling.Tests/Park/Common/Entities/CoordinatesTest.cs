using DddEfteling.Park.Common.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Common.Entities
{
    public class CoordinatesTest
    {
        [Fact]
        public void Construct_CreateCoordinates_ExpectCoordinates()
        {
            Coordinates coordinates = new Coordinates(12.44, 54.28);

            Assert.Equal(12.44, coordinates.Latitude);
            Assert.Equal(54.28, coordinates.Longitude);
            Assert.Equal("12,44;54,28", coordinates.ToString());
        }
    }
}
