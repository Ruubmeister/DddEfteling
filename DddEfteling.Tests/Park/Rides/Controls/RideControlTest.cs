using Castle.Core.Logging;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Controls
{
    public class RideControlTest
    {
        [Fact]
        public void FindRideByName_FindDroomvlucht_ExpectRide()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            RideControl rideControl = new RideControl(realmControl, logger);

            Ride ride = rideControl.FindRideByName("Droomvlucht");
            Assert.NotNull(ride);
            Assert.Equal("Droomvlucht", ride.Name);
        }

        [Fact]
        public void ToMaintenance_SetRideStatusToMaintenance_ExpectMaintenance()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            RideControl rideControl = new RideControl(realmControl, logger);

            Ride ride = rideControl.FindRideByName("Droomvlucht");
            Assert.NotNull(ride);

            rideControl.ToMaintenance(ride);
            Assert.Equal(RideStatus.Maintenance, ride.Status);
        }
    }
}
