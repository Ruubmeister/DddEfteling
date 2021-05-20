using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.ParkTests.Entities;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Geolocation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DddEfteling.ParkTests.Controls
{
    public class LocationServiceTest
    {
        private LocationImpl location1 = new LocationImpl(new Coordinate(5.0, 49.0));
        private LocationImpl location2 = new LocationImpl(new Coordinate(8.0, 51.0));
        private LocationImpl location3 = new LocationImpl(new Coordinate(6.0, 49.5));
        private LocationImpl location4 = new LocationImpl(new Coordinate(7.0, 50.5));
        private LocationImpl location5 = new LocationImpl(new Coordinate(9.0, 51.5));

        private LocationService locationService;

        public LocationServiceTest()
        {
            this.locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), new Random());
        }
        
        [Fact]
        public void CalculateLocationDistances_GivenLocations_ExpectDistancesAdded()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locations.First(loc => loc.Coordinates.Latitude.Equals(5.0));
            
            Assert.Equal(location3.Guid, result.DistanceToOthers.ElementAt(0).Value);
            Assert.Equal(location4.Guid, result.DistanceToOthers.ElementAt(1).Value);
            Assert.Equal(location2.Guid, result.DistanceToOthers.ElementAt(2).Value);

        }
        
        [Fact]
        public void CalculateLocationDistances_GivenEmptyList_ExpectNothingHappens()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>();
            
            locationService.CalculateLocationDistances(locations);

            Assert.NotNull(locations);
        }

        [Fact]
        public void NearestLocation_LocationWithNearbyLocationWithoutExclusion_ExpectNearestLocation()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locationService.NearestLocation(location1, locations, new List<Guid>());
            
            Assert.Equal(location3, result);
            
        }
        
        [Fact]
        public void NearestLocation_NoNearbyLocation_ExpectException()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4};

            Assert.Throws<InvalidOperationException>(() =>
                locationService.NearestLocation(location1, locations, new List<Guid>()));
        }
        
        [Fact]
        public void NearestLocation_LocationWithNearbyLocationWithExclusion_ExpectNearestLocation()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locationService.NearestLocation(location1, locations, 
                new List<Guid>(){ location3.Guid });
            
            Assert.Equal(location4, result);
        }
        
        [Fact]
        public void NextLocation_LocationWithNearbyLocationWithoutExclusion_ExpectNearestLocation()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4, location5};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locationService.NextLocation(location1, locations, new List<Guid>());
            
            Assert.Contains(result, new List<LocationImpl>(){ location2, location3, location4});
        }
        
        [Fact]
        public void NextLocation_LocationWithNearbyLocationWithExclusion_ExpectNearestLocation()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4, location5};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locationService.NextLocation(location1, locations, 
                new List<Guid>() {location3.Guid});
            
            Assert.Contains(result, new List<LocationImpl>(){ location2, location5, location4});
        }
        
        [Fact]
        public void NextLocation_NoNearbyLocation_ExpectNull()
        {
            ConcurrentBag<LocationImpl> locations = new ConcurrentBag<LocationImpl>()
                {location1, location2, location3, location4, location5};
            
            locationService.CalculateLocationDistances(locations);

            LocationImpl result = locationService.NextLocation(location1, locations, 
                new List<Guid>() {location2.Guid, location3.Guid, location4.Guid, location5.Guid});
            
            Assert.Null(result);
        }
    }
}