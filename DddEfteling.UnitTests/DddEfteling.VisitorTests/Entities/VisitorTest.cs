
using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace DddEfteling.VisitorTests.Entities
{
    public class VisitorTest
    {
        private Coordinate startCoordinate = new Coordinate(1.0, 1.0);
        private readonly Random random = new Random();
        private readonly IMediator mediator = new Mock<IMediator>().Object;
        private readonly IOptions<VisitorSettings> settings = new Mock<IOptions<VisitorSettings>>().Object;

        public VisitorTest()
        {
        }

        [Fact]
        public void Construct_createVisitor_expectVisitor()
        {
            DateTime dateOfBirth = DateTime.Now;
            dateOfBirth.Subtract(TimeSpan.FromDays(365 * 20));
            Visitor visitor = new Visitor(dateOfBirth, 1.73, startCoordinate, random, settings);

            Assert.Equal(dateOfBirth, visitor.DateOfBirth);
            Assert.False(visitor.Guid == Guid.Empty);
            Assert.Equal(1.73, visitor.Length);
        }

        [Fact]
        public void WatchFairyTale_LetVisitorWatchFairyTale_ExpectTookTime()
        {
            DateTime start = DateTime.Now;
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            VisitorSettings visitorSettings = new VisitorSettings();
            visitorSettings.FairyTaleMinVisitingSeconds = 3;
            visitorSettings.FairyTaleMaxVisitingSeconds = 5;
            settingsMock.Setup(setting => setting.Value).Returns(visitorSettings);

            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random, settingsMock.Object);
            FairyTaleDto tale = new FairyTaleDto();

            visitor.WatchFairyTale(tale);
        }

        [Fact]

        public void GetLastLocation_HasALocation_ExpectLocation()
        {
            RideDto ride = new RideDto();
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random, settingsMock.Object);

            visitor.AddVisitedLocation(ride);

            Assert.NotEmpty(visitor.VisitedLocations);

            Assert.Equal(ride, visitor.GetLastLocation());
        }

        [Fact]
        public void GetLastLocation_HasNoLocation_ExpectException()
        {
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random, settingsMock.Object);

            Assert.Empty(visitor.VisitedLocations);
            Assert.Null(visitor.GetLastLocation());
        }

        [Fact]
        public void GetLastLocation_SetElevenLocations_ExpectFirstLocationRemovedFromHistory()
        {

            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            DateTime start = DateTime.Now;
            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random, settingsMock.Object);

            RideDto ride1 = new RideDto();
            visitor.AddVisitedLocation(ride1);
            Assert.Equal(ride1, visitor.GetLastLocation());

            for (int i = 1; i <= 10; i++)
            {
                RideDto ride = new RideDto();
                visitor.AddVisitedLocation(ride);
            }

            Assert.NotEmpty(visitor.VisitedLocations);
            Assert.Equal(10, visitor.VisitedLocations.Count);
            Assert.NotEqual(ride1, visitor.GetLastLocation());
            Assert.DoesNotContain(ride1, visitor.VisitedLocations.Values);
        }

        [Fact]
        public void StepInRide_VisitorStepsIntoLine_ExpectHandledData()
        {
            Visitor visitor = new Visitor();
            RideDto ride = new RideDto();

            visitor.CurrentLocation = new Coordinate(1, 2);

            Assert.Empty(visitor.VisitedLocations);

            visitor.StepInRide(ride);

            Assert.NotEmpty(visitor.VisitedLocations);
            Assert.Equal(ride, visitor.GetLastLocation());
            Assert.Null(visitor.TargetLocation);
        }

        [Fact]
        public void WalkToDestination_VisitorStepsIntoLine_ExpectHandledData()
        {
            Visitor visitor = new Visitor();
            RideDto ride = new RideDto();
            ride.Coordinates = new Coordinate(51.64984, 5.04858);
            visitor.CurrentLocation = new Coordinate(51.64937, 5.04797);
            visitor.TargetLocation = ride;

            visitor.WalkToDestination(1.1);

            Assert.True(visitor.CurrentLocation.Latitude < ride.Coordinates.Latitude);
            Assert.True(visitor.CurrentLocation.Latitude > 51.64937);

            Assert.True(visitor.CurrentLocation.Longitude < ride.Coordinates.Longitude);
            Assert.True(visitor.CurrentLocation.Longitude > 5.04797);
        }
    }
}
