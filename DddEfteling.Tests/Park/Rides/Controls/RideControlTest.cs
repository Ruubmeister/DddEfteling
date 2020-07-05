using Castle.Core.Logging;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            IMediator mediator = new Mock<IMediator>().Object;
            Mock<IEmployeeControl> employeeControl = new Mock<IEmployeeControl>();

            employeeControl.Setup(
                employeeControl => employeeControl.GetEmployees(It.IsAny<Ride>())
                ).Returns(new List<Employee>());

            this.rideControl = new RideControl(realmControl, logger, employeeControl.Object, mediator);
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

        [Fact]
        public void OpenAndCloseRides_OpenAllRidesAndCloseAllRides_ExpectAllRidesToBeOpen()
        {

            rideControl.CloseRides();
            Task.Delay(1000).Wait();
            Assert.Empty(rideControl.All().Where(ride => ride.Status.Equals(RideStatus.Open)));
            rideControl.OpenRides();
            Task.Delay(1000).Wait();
            List<Ride> rides = rideControl.All().Where(ride => ride.Status.Equals(RideStatus.Closed)).ToList();
            Assert.Empty((rideControl.All()).Where(ride => ride.Status.Equals(RideStatus.Closed)).ToList());
            Assert.NotEmpty(rideControl.All().Where(ride => ride.Status.Equals(RideStatus.Open)));
            rideControl.CloseRides();
            Task.Delay(1000).Wait();
            Assert.Empty(rideControl.All().Where(ride => ride.Status.Equals(RideStatus.Open)));
            Assert.NotEmpty(rideControl.All().Where(ride => ride.Status.Equals(RideStatus.Closed)));
        }

        [Fact]
        public void Random_GetRandomRide_ExpectRandomRide()
        {
            Ride ride = rideControl.GetRandom();
            Assert.NotNull(ride);
            Assert.Contains(ride, rideControl.All());
        }
    }
}
