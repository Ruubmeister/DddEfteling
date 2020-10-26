using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundary;
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
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Assert.Equal(RideStatus.Open, ride.Status);
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
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            Assert.Equal(RideStatus.Open, ride.Status);
            ride.ToMaintenance();
            Assert.Equal(RideStatus.Maintenance, ride.Status);
        }

        [Fact]
        public void HasVisitor_HasNoVisitors_ExpectFalse()
        {
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open,  coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            VisitorDto visitor = new Mock<VisitorDto>().Object;

            Assert.False(ride.HasVisitor(visitor));
        }

        [Fact]
        public void HasVisitor_HasVisitors_ExpectTrue()
        {
            Coordinate coordinates = new Coordinate(1.22D, 45.44D);
            Ride ride = new Ride(RideStatus.Open, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);

            VisitorDto visitor = new Mock<VisitorDto>().Object;
            ride.AddVisitorToLine(visitor);

            Assert.True(ride.HasVisitor(visitor));
        }
    }
}
