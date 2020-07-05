using DddEfteling.Common.Controls;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DddEfteling.Test.Common.Controls
{
    public class CoordinateExtensionsTest
    {

        [Fact]
        public void GetStepCoordinates_fourCoordinatesInDifferentAngles_expectCorrectCoordinate()
        {
            Coordinate coordinate1 = new Coordinate(51.652613, 5.047075);
            Coordinate coordinateTo1 = new Coordinate(51.652993, 5.047673);
            Coordinate destCoordinate1 = CoordinateExtensions.GetStepCoordinates(coordinate1, coordinateTo1, 10);

            Assert.Equal(51.652675911111345, destCoordinate1.Latitude);
            Assert.Equal(5.0471788553765879, destCoordinate1.Longitude);
        }

        [Fact]
        public void IsInRange_CoordindatesAreInRange_expectTrue()
        {
            Coordinate coordinate1 = new Coordinate(51.648602, 5.052049);
            Coordinate coordinate2 = new Coordinate(51.648725, 5.052186);

            Assert.True(CoordinateExtensions.IsInRange(coordinate1, coordinate2, 17));
        }

        [Fact]
        public void IsInRange_CoordindatesAreNotInRange_expectFalse()
        {
            Coordinate coordinate1 = new Coordinate(51.648602, 5.052049);
            Coordinate coordinate2 = new Coordinate(51.649188, 5.052295);

            Assert.False(CoordinateExtensions.IsInRange(coordinate1, coordinate2, 17));
        }

        [Fact]
        public void GetRelativeBearingToClosestDirection_GiveCoordinates_expectAngles()
        {
            double angle1 = 11.1;
            double angle2 = 89.0;
            double angle3 = 133.0;
            double angle4 = 192.0;
            double angle5 = 259.0;

            Assert.Equal(11.1, CoordinateExtensions.GetRelativeBearingToClosestDirection(angle1));
            Assert.Equal(89.0, CoordinateExtensions.GetRelativeBearingToClosestDirection(angle2));
            Assert.Equal(43.0, CoordinateExtensions.GetRelativeBearingToClosestDirection(angle3));
            Assert.Equal(12.0, CoordinateExtensions.GetRelativeBearingToClosestDirection(angle4));
            Assert.Equal(79.0, CoordinateExtensions.GetRelativeBearingToClosestDirection(angle5));
        }

        [Fact]
        public void GetMaxLatitudeFromBearing_GivenCoordinates_expectCorrectLongitude()
        {
            Coordinate coordinate = new Coordinate(51.647611, 5.050500);

            CoordinateBoundaries boundaries = new CoordinateBoundaries(coordinate, 2.0, DistanceUnit.Meters);

            Assert.Equal(51.647629010716372, CoordinateExtensions.GetMaxLatitudeFromBearing(boundaries, 2));
            Assert.Equal(51.647592989283623, CoordinateExtensions.GetMaxLatitudeFromBearing(boundaries, 91));
            Assert.Equal(51.647629010716372, CoordinateExtensions.GetMaxLatitudeFromBearing(boundaries, 271));
            Assert.Equal(51.647592989283623, CoordinateExtensions.GetMaxLatitudeFromBearing(boundaries, 269));
            Assert.Equal(51.647592989283623, CoordinateExtensions.GetMaxLatitudeFromBearing(boundaries, 180));
        }

        [Fact]
        public void GetMaxLongitudeFromBearing_GivenCoordinates_expectCorrectLongitude()
        {
            Coordinate coordinate = new Coordinate(51.647611, 5.050500);

            CoordinateBoundaries boundaries = new CoordinateBoundaries(coordinate, 2.0, DistanceUnit.Meters);

            Assert.Equal(5.0505290263055267, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 2));
            Assert.Equal(5.0505290263055267, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 91));
            Assert.Equal(5.0505290263055267, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 179));
            Assert.Equal(5.0505290263055267, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 180));
            Assert.Equal(5.0504709736944742, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 181));
            Assert.Equal(5.0504709736944742, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 279));
            Assert.Equal(5.0504709736944742, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 360));
            Assert.Equal(5.0505290263055267, CoordinateExtensions.GetMaxLongitudeFromBearing(boundaries, 0));
        }
    }
}
