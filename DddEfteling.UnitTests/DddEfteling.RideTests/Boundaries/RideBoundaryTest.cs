using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Controls;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundaries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.RideTests.Boundaries
{
    public class RideBoundaryTest
    {
        private readonly Mock<IRideControl> rideControl;
        private readonly RideBoundary rideBoundary;

        public RideBoundaryTest()
        {
            this.rideControl = new Mock<IRideControl>();
            this.rideBoundary = new RideBoundary(rideControl.Object);
        }

        [Fact]
        public void GetRides_RetrieveJson_ExpectsRides()
        {
            this.rideControl.Setup(control => control.All()).Returns(new List<Ride>() { { new Ride() }, { new Ride() } });
            ActionResult<List<RideDto>> rides = rideBoundary.GetRides();

            Assert.NotEmpty(rides.Value);
        }

        [Fact]
        public void GetRandomRide_HasRide_ExpectRide()
        {
            this.rideControl.Setup(control => control.GetRandom()).Returns(new Ride());
            ActionResult<RideDto> tale = rideBoundary.GetRandomRide();

            Assert.NotNull(tale);
        }

        [Fact]
        public void GetNearestFairyTale_HasFairyTales_ExpectFairyTale()
        {
            this.rideControl.Setup(control => control.NearestRide(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(new Ride());
            ActionResult<RideDto> tale = rideBoundary.GetNearestRide(Guid.NewGuid(), "");

            Assert.NotNull(tale);
        }
    }
}
