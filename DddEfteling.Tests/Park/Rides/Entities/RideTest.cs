using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using Moq;
using System;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Entities
{
    public class RideTest
    {
        [Fact]
        public void Construct_CreateRide_ExpectRide()
        {
            Realm realm = new Realm("Test");
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, realm, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Assert.Equal(RideStatus.Open, ride.Status);
            Assert.Equal(realm, ride.Realm);
            Assert.Equal("Rider", ride.Name);
            Assert.Equal(8, ride.MinimumAge);
            Assert.Equal(1.33, ride.MinimumLength);
            Assert.Equal(31, ride.Duration.TotalSeconds);
            Assert.Equal(1.22, ride.Coordinates.Latitude);
            Assert.Equal(45.44, ride.Coordinates.Longitude);
            Assert.Equal(22, ride.MaxPersons);
        }

        [Fact]
        public void ToMaintenance_RideIsSetToMaintenanceStatus_ExpectMaintenanceStatus()
        {
            Realm realm = new Realm("Test");
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, realm, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Assert.Equal(RideStatus.Open, ride.Status);
            ride.ToMaintenance();
            Assert.Equal(RideStatus.Maintenance, ride.Status);
        }

        [Fact]
        public void HasVisitor_HasNoVisitors_ExpectFalse()
        {
            Realm realm = new Realm("Test");
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, realm, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Visitor visitor = new Mock<Visitor>().Object;

            Assert.False(ride.HasVisitor(visitor));
        }

        [Fact]
        public void HasVisitor_HasVisitors_ExpectTrue()
        {
            Realm realm = new Realm("Test");
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, realm, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Visitor visitor = new Mock<Visitor>().Object;
            ride.AddVisitorToLine(visitor);

            Assert.True(ride.HasVisitor(visitor));
        }
    }
}
