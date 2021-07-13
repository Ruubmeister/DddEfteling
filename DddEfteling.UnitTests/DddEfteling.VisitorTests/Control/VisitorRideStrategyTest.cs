using System;
using System.Collections.Generic;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Moq;
using Xunit;

namespace DddEfteling.VisitorTests.Control
{
    public class VisitorRideStrategyTest
    {
        private Mock<IEventProducer> eventProducerMock = new Mock<IEventProducer>();
        private Mock<IRideClient> rideClientMock = new Mock<IRideClient>();
        
        [Fact]
        public void StartLocationActivity_GivenCorrectData_ExpectCorrectCalls()
        {
            Visitor visitor = new Visitor();
            RideDto rideDto = new RideDto();
            visitor.TargetLocation = rideDto;
            VisitorRideStrategy strategy =
                new VisitorRideStrategy(eventProducerMock.Object, rideClientMock.Object);
            
            strategy.StartLocationActivity(visitor);
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() },
                {"Ride", rideDto.Guid.ToString()}
            };

            Event producedEvent = new Event(EventType.StepInRideLine, EventSource.Visitor, eventPayload);
            eventProducerMock.Verify(producer => producer.Produce(It.Is<Event>( a => a.Equals(producedEvent))), Times.Once);
        }

        [Fact]
        public void SetNewLocation_GivenVisitorHasNoHistory_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            VisitorRideStrategy strategy =
                new VisitorRideStrategy(eventProducerMock.Object, rideClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            rideClientMock.Verify(client => client.GetRandomRide(), Times.Once);
            rideClientMock.Verify(client => client.GetNextLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasFairyTaleAsLatestLocation_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            RideDto location = new RideDto();
            location.LocationType = LocationType.FAIRYTALE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorRideStrategy strategy =
                new VisitorRideStrategy(eventProducerMock.Object, rideClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            rideClientMock.Verify(client => client.GetRandomRide(), Times.Once);
            rideClientMock.Verify(client => client.GetNextLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
            
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasrideAsLatestLocation_ExpectClosestRequested()
        {
            
            Visitor visitor = new Visitor();
            RideDto location = new RideDto();
            rideClientMock.Setup(mock => 
                mock.GetNextLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(location);
            location.LocationType = LocationType.RIDE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorRideStrategy strategy =
                new VisitorRideStrategy(eventProducerMock.Object, rideClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            rideClientMock.Verify(client => client.GetRandomRide(), Times.Never);
            rideClientMock.Verify(client => client.GetNextLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Once);
        }
    }
}