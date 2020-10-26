using Castle.Core.Logging;
using DddEfteling.Rides.Boundary;
using DddEfteling.Rides.Controls;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundary;
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
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            IVisitorClient visitorClient = Mock.Of<IVisitorClient>();
            Mock<IEmployeeClient> employeeClient = new Mock<IEmployeeClient>();

            /*employeeClient.Setup(
                employeeControl => employeeControl.GetEmployees(It.IsAny<Ride>())
                ).Returns(new List<Employee>());*/

            this.rideControl = new RideControl(logger, eventProducer, employeeClient.Object, visitorClient);
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
