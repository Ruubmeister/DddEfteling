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
    public class VisitorStandStrategyTest
    {
        private Mock<IEventProducer> eventProducerMock = new Mock<IEventProducer>();
        private Mock<IStandClient> standClientMock = new Mock<IStandClient>();
        
        [Fact]
        public void StartLocationActivity_GivenCorrectData_ExpectCorrectCalls()
        {
            Visitor visitor = new Visitor();
            StandDto standDto = new StandDto();
            visitor.TargetLocation = standDto;
            string ticket = "ticket";
            
            VisitorStandStrategy strategy =
                new VisitorStandStrategy(eventProducerMock.Object, standClientMock.Object);
            standClientMock.Setup(client => client.OrderDinner(It.IsAny<Guid>(), It.IsAny<List<string>>()))
                .Returns(ticket);
            standClientMock.Setup(client => client.GetStand(It.IsAny<Guid>())).Returns(standDto);

            strategy.StartLocationActivity(visitor);
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() },
                {"Ticket", ticket}
            };

            Event producedEvent = new Event(EventType.WaitingForOrder, EventSource.Visitor, eventPayload);
            eventProducerMock.Verify(producer => producer.Produce(It.Is<Event>( a => a.Equals(producedEvent))), Times.Once);
        }

        [Fact]
        public void SetNewLocation_GivenVisitorHasNoHistory_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            VisitorStandStrategy strategy =
                new VisitorStandStrategy(eventProducerMock.Object, standClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            standClientMock.Verify(client => client.GetRandomStand(), Times.Once);
            standClientMock.Verify(client => client.GetNewStandLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasFairyTaleAsLatestLocation_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            StandDto location = new StandDto();
            location.LocationType = LocationType.FAIRYTALE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorStandStrategy strategy =
                new VisitorStandStrategy(eventProducerMock.Object, standClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            standClientMock.Verify(client => client.GetRandomStand(), Times.Once);
            standClientMock.Verify(client => client.GetNewStandLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
            
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasstandAsLatestLocation_ExpectClosestRequested()
        {
            
            Visitor visitor = new Visitor();
            StandDto location = new StandDto();
            standClientMock.Setup(mock => 
                mock.GetNewStandLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(location);
            location.LocationType = LocationType.RIDE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorStandStrategy strategy =
                new VisitorStandStrategy(eventProducerMock.Object, standClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            standClientMock.Verify(client => client.GetRandomStand(), Times.Never);
            standClientMock.Verify(client => client.GetNewStandLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Once);
        }
        
    }
}