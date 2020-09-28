using Geolocation;
using System;

namespace DddEfteling.Shared.Controls
{
    public static class CoordinateExtensions
    {
        private static double radialNumber = Math.PI / 180;
        public static bool IsInRange(Coordinate from, Coordinate to, Double distance)
        {
            return distance > GeoCalculator.GetDistance(from, to, 1, DistanceUnit.Meters);
        }

        public static Coordinate GetStepCoordinates(Coordinate from, Coordinate to, Double distance)
        {
            double bearing = GeoCalculator.GetBearing(from, to);

            double angle = GetRelativeBearingToClosestDirection(bearing);

            (double latitudeFactor, double longitudeFactor) = CorrectionFactors(angle, bearing);

            CoordinateBoundaries boundaries = new CoordinateBoundaries(from, distance, DistanceUnit.Meters);

            double maxLongitude = GetMaxLongitudeFromBearing(boundaries, bearing);
            double maxLatitude = GetMaxLatitudeFromBearing(boundaries, bearing);

            double newLongitude = from.Longitude + (maxLongitude - from.Longitude) * longitudeFactor;
            double newLatitude = from.Latitude + (maxLatitude - from.Latitude) * latitudeFactor;

            return new Coordinate(newLatitude, newLongitude);
        }

        private static (double latitudeFactor, double longitudeFactor) CorrectionFactors(double angle, double bearing)
        {

            double factor1 = Math.Sin(angle * radialNumber);
            double factor2 = Math.Cos(angle * radialNumber);

            if (Math.Floor(bearing / 90) % 2 ==0)
            {
                return (latitudeFactor: factor2, longitudeFactor: factor1);
            }

            return (latitudeFactor: factor1, longitudeFactor: factor2);
        }

        public static double GetRelativeBearingToClosestDirection(double absoluteBearing)
        {
            //What this function does, is remove 90, 180 or 270 so the angle is between 0 and 89.9 for calculations
            return absoluteBearing - (Math.Floor(absoluteBearing / 90) * 90);
        }

        public static double GetMaxLongitudeFromBearing(CoordinateBoundaries boundary, double bearing)
        {
            if (bearing <= 180)
            {
                return boundary.MaxLongitude;
            }
            else
            {
                return boundary.MinLongitude;
            }
        }

        public static double GetMaxLatitudeFromBearing(CoordinateBoundaries boundary, double bearing)
        {
            if (bearing >= 90 && bearing < 270)
            {
                return boundary.MinLatitude;
            }
            else
            {
                return boundary.MaxLatitude;
            }
        }
    }
}
