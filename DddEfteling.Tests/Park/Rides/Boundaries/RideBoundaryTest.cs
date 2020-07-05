using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Boundary;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
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
        IMediator mediator;
        IEmployeeControl employeeControl;

        public FairyTaleBoundaryTest()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            this.rideControl = new RideControl(realmControl, logger, employeeControl, mediator);
            this.rideBoundary = new RideBoundary(rideControl);
            this.mediator = new Mock<IMediator>().Object;
            this.employeeControl = new Mock<IEmployeeControl>().Object;
        }

        [Fact]
        public void GetRides_RetrieveJson_ExpectsRides()
        {
            ActionResult<List<Ride>> rides = rideBoundary.GetRides();

            Assert.NotEmpty(rides.Value);
        }
    }
}
