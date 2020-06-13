using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Rides.Boundary;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
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
            IRealmControl realmControl = new RealmControl();
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            this.rideControl = new RideControl(realmControl, logger);
            this.rideBoundary = new RideBoundary(rideControl);
        }

        [Fact]
        public void GetRides_RetrieveJson_ExpectsRides()
        {
            ActionResult<List<Ride>> rides = rideBoundary.GetRides();

            Assert.NotEmpty(rides.Value);
        }
    }
}
