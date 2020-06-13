using Castle.Core.Logging;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Controls
{
    public class RideControlTest
    {
        RideControl rideControl;

        public RideControlTest()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            this.rideControl = new RideControl(realmControl, logger);
        }

        [Fact]
        public void FindRideByName_FindDroomvlucht_ExpectRide()
        {
            Ride ride = rideControl.FindRideByName("Droomvlucht");
            Assert.NotNull(ride);
            Assert.Equal("Droomvlucht", ride.Name);
        }

        [Fact]
        public void ToMaintenance_SetRideStatusToMaintenance_ExpectMaintenance()
        {
            Ride ride = rideControl.FindRideByName("Droomvlucht");
            Assert.NotNull(ride);

            rideControl.ToMaintenance(ride);
            Assert.Equal(RideStatus.Maintenance, ride.Status);
        }

        [Fact]
        public void All_GetAllRides_ExpectRides()
        {
            List<Ride> rides = rideControl.All();
            Assert.NotEmpty(rides);
            Assert.Single(rides.Where(ride => ride.Name.Equals("Python")));
        }
    }
}
