﻿using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Controls;
using DddEfteling.Rides.Entities;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DddEfteling.RideTests.Controls
{
    public class RideControlTest
    {
        private readonly RideControl rideControl;
        private readonly VisitorDto visitor = new VisitorDto() { Guid = Guid.NewGuid() };
        private readonly Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

        public RideControlTest()
        {
            ILogger<RideControl> logger = Mock.Of<ILogger<RideControl>>();
            Mock<IVisitorClient> visitorClientMock = new Mock<IVisitorClient>();
            Mock<IEmployeeClient> employeeClient = new Mock<IEmployeeClient>();

            visitorClientMock.Setup(client => client.GetVisitor(It.IsAny<Guid>())).Returns(visitor);

            /*employeeClient.Setup(
                employeeControl => employeeControl.GetEmployees(It.IsAny<Ride>())
                ).Returns(new List<Employee>());*/

            this.rideControl = new RideControl(logger, eventProducer.Object, employeeClient.Object, visitorClientMock.Object);
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


        [Fact]
        public void NearestRide_GetNearestFromDeVliegendeHollanderWithoutExclusions_ExpectJorisEnDeDraak()
        {
            Ride ride = this.rideControl.All().First(tale => tale.Name.Equals("De Vliegende Hollander"));

            Ride closest = this.rideControl.NearestRide(ride.Guid, new List<System.Guid>());
            Assert.NotNull(closest);
            Assert.Equal("Joris en de Draak", closest.Name);
        }

        [Fact]
        public void NearestRide_GetNearestFromDeVliegendeHollanderWithExclusions_ExpectPython()
        {
            Ride tale = this.rideControl.All().First(tale => tale.Name.Equals("De Vliegende Hollander"));

            List<Ride> excludedTales = this.rideControl.All().Where(tale => tale.Name.Equals("Joris en de Draak") || tale.Equals("Baron 1898") || tale.Equals("Polka Marina")).ToList();

            Ride closest = this.rideControl.NearestRide(tale.Guid, excludedTales.ConvertAll(tale => tale.Guid));
            Assert.NotNull(closest);
            Assert.Equal("Python", closest.Name);
        }

        [Fact]
        public void HandleVisitorSteppingInRideLine_VisitorsStepsIntoOpenRideLine_ExpectVisitorInLine()
        {

            Ride ride = rideControl.GetRandom();
            ride.ToOpen();

            Assert.False(ride.HasVisitor(visitor));

            rideControl.HandleVisitorSteppingInRideLine(visitor.Guid, ride.Guid);

            Assert.True(ride.HasVisitor(visitor));
            ride.ToClosed();
        }

        [Fact]
        public void HandleVisitorSteppingInRideLine_VisitorsStepsIntoClosedRideLine_ExpectVisitorNotInLine()
        {

            Ride ride = rideControl.GetRandom();
            ride.ToClosed();

            Assert.False(ride.HasVisitor(visitor));

            rideControl.HandleVisitorSteppingInRideLine(visitor.Guid, ride.Guid);

            Assert.False(ride.HasVisitor(visitor));
        }

        [Fact]
        public void HandleOpenRides_RideOpenWithVisitorAndRunEnded_ExpectBoardingAndUnboardingVisitor()
        {
            Ride ride = rideControl.GetRandom();
            ride.ToOpen();
            ride.EndTime = DateTime.Now;

            Assert.False(ride.HasVisitor(visitor));

            rideControl.HandleVisitorSteppingInRideLine(visitor.Guid, ride.Guid);

            Assert.True(ride.HasVisitor(visitor));

            rideControl.HandleOpenRides();
            Assert.True(ride.HasVisitor(visitor));
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);

            rideControl.HandleOpenRides();
            Assert.True(ride.HasVisitor(visitor));
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);

            ride.EndTime = DateTime.Now;

            rideControl.HandleOpenRides();
            Assert.False(ride.HasVisitor(visitor));
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Once);
        }
    }
}