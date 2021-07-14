
using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace DddEfteling.VisitorTests.Entities
{
    public class VisitorTest
    {
        private Coordinate startCoordinate = new Coordinate(1.0, 1.0);
        private readonly Random random = new Random();
        private readonly IMediator mediator = new Mock<IMediator>().Object;

        public VisitorTest()
        {
        }

        [Fact]
        public void Construct_createVisitor_expectVisitor()
        {
            DateTime dateOfBirth = DateTime.Now;
            dateOfBirth.Subtract(TimeSpan.FromDays(365 * 20));
            Visitor visitor = new Visitor(dateOfBirth, 1.73, startCoordinate, random);

            Assert.Equal(dateOfBirth, visitor.DateOfBirth);
            Assert.False(visitor.Guid == Guid.Empty);
            Assert.Equal(1.73, visitor.Length);
        }

        [Fact]
        public void DoActivity_LetVisitorDoActivity_ExpectTookTime()
        {
            DateTime start = DateTime.Now;
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            VisitorSettings visitorSettings = new VisitorSettings();
            visitorSettings.FairyTaleMinVisitingSeconds = 3;
            visitorSettings.FairyTaleMaxVisitingSeconds = 5;
            settingsMock.Setup(setting => setting.Value).Returns(visitorSettings);

            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random);
            FairyTaleDto tale = new FairyTaleDto();

            visitor.DoActivity(tale);
        }

        [Fact]

        public void GetLastLocation_HasALocation_ExpectLocation()
        {
            RideDto ride = new RideDto();
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random);

            visitor.AddVisitedLocation(ride);

            Assert.NotEmpty(visitor.VisitedLocations);

            Assert.Equal(ride, visitor.GetLastLocation());
        }

        [Fact]
        public void GetLastLocation_HasNoLocation_ExpectException()
        {
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random);

            Assert.Empty(visitor.VisitedLocations);
            Assert.Null(visitor.GetLastLocation());
        }

        [Fact]
        public void GetLastLocation_SetElevenLocations_ExpectFirstLocationRemovedFromHistory()
        {

            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random);

            RideDto ride1 = new RideDto();
            visitor.AddVisitedLocation(ride1);
            Assert.Equal(ride1, visitor.GetLastLocation());

            for (int i = 1; i <= 10; i++)
            {
                RideDto ride = new RideDto();
                visitor.AddVisitedLocation(ride);
                Thread.Sleep(10);
            }

            Assert.NotEmpty(visitor.VisitedLocations);
            Assert.Equal(10, visitor.VisitedLocations.Count);
            Assert.NotEqual(ride1, visitor.GetLastLocation());
            Assert.DoesNotContain(ride1, visitor.VisitedLocations.Values);
        }

        [Fact]
        public void WalkToDestination_VisitorStepsIntoLine_ExpectHandledData()
        {
            Visitor visitor = new Visitor();
            RideDto ride = new RideDto();
            ride.Coordinates = new Coordinate(51.64984, 5.04858);
            visitor.CurrentLocation = new Coordinate(51.64937, 5.04797);
            visitor.TargetLocation = ride;
            visitor.NextStepDistance = 1.1;

            visitor.WalkToDestination();

            Assert.True(visitor.CurrentLocation.Latitude < ride.Coordinates.Latitude);
            Assert.True(visitor.CurrentLocation.Latitude > 51.64937);

            Assert.True(visitor.CurrentLocation.Longitude < ride.Coordinates.Longitude);
            Assert.True(visitor.CurrentLocation.Longitude > 5.04797);
        }

        [Fact]
        public void PickStandProducts_GivenStandWithProducts_ExpectProducts()
        {
            var meals = new List<string>() {"Product 1", "Product 2"};
            var drinks = new List<string>() {"Drink 1", "Drink 2"};
            
            Visitor visitor = new Visitor();
            StandDto standDto = new StandDto(Guid.NewGuid(), "Stand", new Coordinate(), meals, drinks);

            List<string> products = visitor.PickStandProducts(standDto);

            bool inMeals = false;
            bool inDrinks = false;
            
            foreach(var product in products)
            {
                if (standDto.Drinks.Contains(product))
                {
                    inDrinks = true;
                }else if (standDto.Meals.Contains(product))
                {
                    inMeals = true;
                }
            }
            Assert.Equal(2, products.Count);
            Assert.True(inDrinks);
            Assert.True(inMeals);
        }
        
        [Fact]
        public void PickStandProducts_GivenStandWithMeals_ExpectMeal()
        {
            var meals = new List<string>() {"Product 1", "Product 2"};
            var drinks = new List<string>();
            
            Visitor visitor = new Visitor();
            StandDto standDto = new StandDto(Guid.NewGuid(), "Stand", new Coordinate(), meals, drinks);

            List<string> products = visitor.PickStandProducts(standDto);

            bool inMeals = false;
            bool inDrinks = false;
            
            foreach(var product in products)
            {
                if (standDto.Drinks.Contains(product))
                {
                    inDrinks = true;
                }else if (standDto.Meals.Contains(product))
                {
                    inMeals = true;
                }
            }
            Assert.Single( products);
            Assert.False(inDrinks);
            Assert.True(inMeals);
        }
        
        [Fact]
        public void PickStandProducts_GivenStandWithDrinks_ExpectDrink()
        {
            var meals = new List<string>();
            var drinks = new List<string>() {"Product 1", "Product 2"};
            
            Visitor visitor = new Visitor();
            StandDto standDto = new StandDto(Guid.NewGuid(), "Stand", new Coordinate(), meals, drinks);

            List<string> products = visitor.PickStandProducts(standDto);

            bool inMeals = false;
            bool inDrinks = false;
            
            foreach(var product in products)
            {
                if (standDto.Drinks.Contains(product))
                {
                    inDrinks = true;
                }else if (standDto.Meals.Contains(product))
                {
                    inMeals = true;
                }
            }
            
            Assert.Single(products);
            Assert.True(inDrinks);
            Assert.False(inMeals);
        }
        
        [Fact]
        public void PickStandProducts_GivenStandWithoutDrinks_ExpectEmptyList()
        {
            var meals = new List<string>();
            var drinks = new List<string>();
            
            Visitor visitor = new Visitor();
            StandDto standDto = new StandDto(Guid.NewGuid(), "Stand", new Coordinate(), meals, drinks);

            List<string> products = visitor.PickStandProducts(standDto);

            Assert.Empty(products);
        }
    }
}
