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
    public class VisitorFairyTaleStrategyTest
    {
        private Mock<IEventProducer> eventProducerMock = new Mock<IEventProducer>();
        private Mock<IFairyTaleClient> fairyTaleClientMock = new Mock<IFairyTaleClient>();
        
        [Fact]
        public void StartLocationActivity_GivenCorrectData_ExpectCorrectCalls()
        {
            Visitor visitor = new Visitor();
            FairyTaleDto fairyTaleDto = new FairyTaleDto();
            visitor.TargetLocation = fairyTaleDto;
            VisitorFairyTaleStrategy strategy =
                new VisitorFairyTaleStrategy(eventProducerMock.Object, fairyTaleClientMock.Object);
            
            strategy.StartLocationActivity(visitor);
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() },
                {"FairyTale", fairyTaleDto.Guid.ToString()}
            };

            Event producedEvent = new Event(EventType.ArrivedAtFairyTale, EventSource.Visitor, eventPayload);
            eventProducerMock.Verify(producer => producer.Produce(It.Is<Event>( a => a.Equals(producedEvent))), Times.Once);
        }

        [Fact]
        public void SetNewLocation_GivenVisitorHasNoHistory_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            VisitorFairyTaleStrategy strategy =
                new VisitorFairyTaleStrategy(eventProducerMock.Object, fairyTaleClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            fairyTaleClientMock.Verify(client => client.GetRandomFairyTale(), Times.Once);
            fairyTaleClientMock.Verify(client => client.GetNewFairyTaleLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasRideAsLatestLocation_ExpectRandomRequested()
        {
            Visitor visitor = new Visitor();
            RideDto location = new RideDto();
            location.LocationType = LocationType.RIDE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorFairyTaleStrategy strategy =
                new VisitorFairyTaleStrategy(eventProducerMock.Object, fairyTaleClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            fairyTaleClientMock.Verify(client => client.GetRandomFairyTale(), Times.Once);
            fairyTaleClientMock.Verify(client => client.GetNewFairyTaleLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Never);
            
        }
        
        [Fact]
        public void SetNewLocation_GivenVisitorHasFairyTaleAsLatestLocation_ExpectClosestRequested()
        {
            
            Visitor visitor = new Visitor();
            FairyTaleDto location = new FairyTaleDto();
            fairyTaleClientMock.Setup(mock => 
                mock.GetNewFairyTaleLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(location);
            location.LocationType = LocationType.FAIRYTALE;
            visitor.VisitedLocations.Add(DateTime.Now, location);
            VisitorFairyTaleStrategy strategy =
                new VisitorFairyTaleStrategy(eventProducerMock.Object, fairyTaleClientMock.Object);
            strategy.SetNewLocation(visitor);
            
            fairyTaleClientMock.Verify(client => client.GetRandomFairyTale(), Times.Never);
            fairyTaleClientMock.Verify(client => client.GetNewFairyTaleLocation(It.IsAny<Guid>(),
                It.IsAny<List<Guid>>()), Times.Once);
        }
    }
}