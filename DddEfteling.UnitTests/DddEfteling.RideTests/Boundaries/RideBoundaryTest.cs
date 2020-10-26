using DddEfteling.Rides.Boundary;
using DddEfteling.Rides.Controls;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundary;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Boundaries
{
    public class FairyTaleBoundaryTest
    {
        IRideControl rideControl;
        RideBoundary rideBoundary;

        public FairyTaleBoundaryTest()
        {
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            IVisitorClient visitorClient = Mock.Of<IVisitorClient>();
            IEmployeeClient employeeClient = Mock.Of<IEmployeeClient>();
            this.rideControl = new RideControl(logger, eventProducer, employeeClient, visitorClient);
            this.rideBoundary = new RideBoundary(rideControl);
        }

        [Fact]
        public void GetRides_RetrieveJson_ExpectsRides()
        {
            ActionResult<List<RideDto>> rides = rideBoundary.GetRides();

            Assert.NotEmpty(rides.Value);
        }
    }
}
