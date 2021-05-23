using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace DddEfteling.ParkTests.Entities
{
    public class LocationRepositoryTest
    {
        private LocationRepository<LocationImpl> repo;
        private Mock<ILocationService> locationService = new Mock<ILocationService>();
        public LocationRepositoryTest()
        {
            LocationConverter<LocationImpl> converter = new LocationConverter<LocationImpl>((x) => new LocationImpl(x));
            this.repo = new LocationRepository<LocationImpl>(locationService.Object, converter);
        }

        [Fact]
        public void FindByName_LocationExists_ExpectLocation()
        {
            LocationImpl location = repo.FindByName("Test 1");
            Assert.NotNull(location);
            Assert.Equal("Test 1", location.Name);
        }
        
        [Fact]
        public void FindByName_LocationDoesNotExists_ExpectNull()
        {
            LocationImpl location = repo.FindByName("Test 3");
            Assert.Null(location);
        }

        [Fact]
        public void All_GivenRepoHasTwoLocations_ExpectTwoLocations()
        {
            ConcurrentBag<LocationImpl> locations = repo.All();
            Assert.NotEmpty(locations);
            Assert.Equal(2, locations.Count);
        }
        
        [Fact]
        public void AllAsList_GivenRepoHasTwoLocations_ExpectTwoLocations()
        {
            List<LocationImpl> locations = repo.AllAsList();
            Assert.NotEmpty(locations);
            Assert.Equal(2, locations.Count);
        }
        
        [Fact]
        public void FindByGuid_GivenRepoHasLocation_ExpectSameLocation()
        {
            LocationImpl location = repo.FindByName("Test 1");
            LocationImpl result = repo.FindByGuid(location.Guid);
            
            Assert.NotNull(result);
            Assert.Equal(location, result);
        }
        
        [Fact]
        public void NearestLocation_GivenCorrectLocation_ExpectLocation()
        {
            LocationImpl location = repo.FindByName("Test 1");
            LocationImpl newLocation = new LocationImpl();

            locationService.Setup(service => service.NearestLocation(It.IsAny<LocationImpl>(),
                It.IsAny<ConcurrentBag<LocationImpl>>(), It.IsAny<List<Guid>>())).Returns(newLocation);

            LocationImpl result = repo.NearestLocation(location.Guid, new List<Guid>());
            
            Assert.Equal(newLocation, result);
        }
        
        [Fact]
        public void NearestLocation_GivenUnknownLocation_ExpectException()
        {
            Assert.Throws<InvalidOperationException>(() => repo.NearestLocation(Guid.NewGuid(), new List<Guid>()));
        }

        [Fact]
        public void NextLocation_GivenCorrectLocation_ExpectLocation()
        {
            LocationImpl location = repo.FindByName("Test 1");
            LocationImpl newLocation = new LocationImpl();

            locationService.Setup(service => service.NextLocation(It.IsAny<LocationImpl>(),
                It.IsAny<ConcurrentBag<LocationImpl>>(), It.IsAny<List<Guid>>())).Returns(newLocation);

            LocationImpl result = repo.NextLocation(location.Guid, new List<Guid>());
            
            Assert.Equal(newLocation, result);
        }
        
        
        [Fact]
        public void NextLocation_GivenUnknownLocation_ExpectException()
        {
            Assert.Throws<InvalidOperationException>(() => repo.NextLocation(Guid.NewGuid(), new List<Guid>()));
        }

        [Fact]
        public void GetRandom_GivenListHasLocations_ExpectRandomLocation()
        {
            List<LocationImpl> locations = repo.AllAsList();
            LocationImpl result = repo.GetRandom();
            
            Assert.NotNull(result);
            Assert.Contains(result, locations);

        }
    }
}