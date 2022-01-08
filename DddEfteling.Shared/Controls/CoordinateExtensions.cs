using Geolocation;
using System;

namespace DddEfteling.Shared.Controls
{
    public static class CoordinateExtensions
    {
        private static readonly double radialNumber = Math.PI / 180;
        public static bool IsInRange(Coordinate from, Coordinate to, double distance)
        {
            return distance > GeoCalculator.GetDistance(from, to, 1, DistanceUnit.Meters);
        }

        public static Coordinate GetStepCoordinates(Coordinate from, Coordinate to, double distance)
        {
            var bearing = GeoCalculator.GetBearing(from, to);

            var angle = GetRelativeBearingToClosestDirection(bearing);

            var (latitudeFactor, longitudeFactor) = CorrectionFactors(angle, bearing);

            var boundaries = new CoordinateBoundaries(from, distance, DistanceUnit.Meters);

            var maxLongitude = GetMaxLongitudeFromBearing(boundaries, bearing);
            var maxLatitude = GetMaxLatitudeFromBearing(boundaries, bearing);

            var newLongitude = from.Longitude + (maxLongitude - from.Longitude) * longitudeFactor;
            var newLatitude = from.Latitude + (maxLatitude - from.Latitude) * latitudeFactor;

            return new Coordinate(newLatitude, newLongitude);
        }

        private static (double latitudeFactor, double longitudeFactor) CorrectionFactors(double angle, double bearing)
        {

            var factor1 = Math.Sin(angle * radialNumber);
            var factor2 = Math.Cos(angle * radialNumber);

            return (Math.Floor(bearing / 90) % 2 == 0)
                ? (latitudeFactor: factor2, longitudeFactor: factor1)
                : (latitudeFactor: factor1, longitudeFactor: factor2);
        }

        public static double GetRelativeBearingToClosestDirection(double absoluteBearing)
        {
            //What this function does, is remove 90, 180 or 270 so the angle is between 0 and 89.9 for calculations
            return absoluteBearing - (Math.Floor(absoluteBearing / 90) * 90);
        }

        public static double GetMaxLongitudeFromBearing(CoordinateBoundaries boundary, double bearing)
        {
            return bearing <= 180 ? boundary.MaxLongitude : boundary.MinLongitude;
        }

        public static double GetMaxLatitudeFromBearing(CoordinateBoundaries boundary, double bearing)
        {
            return bearing >= 90 && bearing < 270 ? boundary.MinLatitude : boundary.MaxLatitude;
        }
    }
}
