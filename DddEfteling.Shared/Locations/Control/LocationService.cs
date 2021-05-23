using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Shared.Controls
{
    public class LocationService: ILocationService
    {
        private readonly ILogger<LocationService> logger;
        private readonly Random random;

        public LocationService(ILogger<LocationService> logger, Random random)
        {
            this.logger = logger;
            this.random = random;
        }
        
        public void CalculateLocationDistances<T>(ConcurrentBag<T> locations) where T: Location
        {
            foreach (T location in locations)
            {
                foreach (T toLocation in locations)
                {
                    if (location.Equals(toLocation))
                    {
                        continue;
                    }

                    location.AddDistanceToOthers(location.GetDistanceTo(toLocation), toLocation.Guid);
                    logger.LogDebug($"Calculated distance from {location.Name} to {toLocation.Name}");
                }
            }
            logger.LogDebug("Calculated distance to all fairy tales");
        }
        
        public T NearestLocation<T>(T location, ConcurrentBag<T> locations, List<Guid> exclusionList) where T: Location
        {
            Guid nextLocation = location.DistanceToOthers.First(keyVal => !exclusionList.Contains(keyVal.Value)).Value;

            return locations.First(x => x.Guid.Equals(nextLocation));
        }

        public T NextLocation<T>(T location, ConcurrentBag<T> locations, List<Guid> exclusionList) where T: Location
        {
            
            try
            {
                List<KeyValuePair<double, Guid>> locationsToPick = location.DistanceToOthers.Where(keyVal => !exclusionList.Contains(keyVal.Value)).Take(3).ToList();
                Guid nextLocation = locationsToPick.ElementAt(random.Next(locationsToPick.Count)).Value;
                return locations.First(tmpLocation => tmpLocation.Guid.Equals(nextLocation));
            }
            catch (Exception e)
            {
                logger.LogWarning("Something went wrong when getting a next location from origin {name}: {Exception}", location.Name, e);
            }

            return null;
        }
    }

    public interface ILocationService
    {
        public void CalculateLocationDistances<T>(ConcurrentBag<T> locations) where T: Location;

        public T NearestLocation<T>(T location, ConcurrentBag<T> locations, List<Guid> exclusionList) where T: Location;

        public T NextLocation<T>(T location, ConcurrentBag<T> locations, List<Guid> exclusionList) where T: Location;
    }
}