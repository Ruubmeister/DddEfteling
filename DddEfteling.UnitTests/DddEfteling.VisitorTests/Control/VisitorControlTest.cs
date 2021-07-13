using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace DddEfteling.VisitorTests.Control
{
    public class VisitorControlTest
    {
        private readonly IMediator mediator = new Mock<IMediator>().Object;
        private readonly ILogger<VisitorControl> logger = new Mock<ILogger<VisitorControl>>().Object;
        private readonly IOptions<VisitorSettings> settings = new Mock<IOptions<VisitorSettings>>().Object;
        private readonly Mock<IVisitorRepository> repo = new Mock<IVisitorRepository>();
        private readonly Mock<IVisitorMovementService> visitorMovementService = new Mock<IVisitorMovementService>();
        private readonly Mock<ILocationTypeStrategy> locationTypeStrategyMock = new Mock<ILocationTypeStrategy>();
        private readonly IFairyTaleClient fairyTaleClient;
        private readonly IRideClient rideClient;
        private readonly IEventProducer eventProducer;
        private readonly IStandClient standClient;

        public VisitorControlTest()
        {
            this.fairyTaleClient = Mock.Of<IFairyTaleClient>();
            this.rideClient = Mock.Of<IRideClient>();
            this.eventProducer = Mock.Of<IEventProducer>();
            this.standClient = Mock.Of<IStandClient>();
        }

        [Fact]
        public void AddVisitors_AddThree_ExpectThree()
        {
            repo.Setup(repo => repo.AddVisitors(It.IsAny<int>())).Returns(new List<Visitor>());
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.AddVisitors(3);
            repo.Verify(repo => repo.AddVisitors(3));

        }

        [Fact]
        public void NotifyIdleVisitor_GuidGiven_ExpectMediatorCall()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>();

            VisitorControl visitorControl = new VisitorControl(mediatorMock.Object, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);
            Guid guid = Guid.NewGuid();
            visitorControl.NotifyIdleVisitor(guid);

            mediatorMock.Verify(mock =>
                mock.Publish(It.Is<VisitorEvent>(visitorEvent => visitorEvent.VisitorGuid.Equals(guid)),
                    CancellationToken.None));
        }
        
        [Fact]
        public void NotifyOrderReady_OrderExists_ExpectVisitorIdle()
        {
            FairyTaleDto fairyTaleDto = new FairyTaleDto();

            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());

            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            Visitor input = new Visitor();
            repo.Setup(repo => repo.AddVisitors(It.IsAny<int>())).Returns(new List<Visitor>() {input});
            repo.Setup(repo => repo.All()).Returns(new List<Visitor>() {input});
            repo.Setup(repo => repo.GetVisitor(input.Guid)).Returns(input);
            
            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);

            Guid guid = Guid.NewGuid();

            visitorControl.AddVisitors(1);

            Visitor visitor = visitorControl.All().First();

            visitorControl.VisitorsWaitingForOrder.TryAdd(guid.ToString(), visitor.Guid);
            visitorControl.NotifyOrderReady(guid.ToString());
            Assert.True(visitor.AvailableAt < DateTime.Now && visitor.AvailableAt > DateTime.Now - TimeSpan.FromSeconds(5));

        }

        [Fact]
        public void NotifyOrderReady_OrderDoesNotExists_ExpectNoVisitorIdle()
        {
            FairyTaleDto fairyTaleDto = new FairyTaleDto();

            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());

            Visitor input = new Visitor();
            repo.Setup(repo => repo.AddVisitors(It.IsAny<int>())).Returns(new List<Visitor>() {input});
            repo.Setup(repo => repo.All()).Returns(new List<Visitor>() {input});
            repo.Setup(repo => repo.GetVisitor(input.Guid)).Returns(input);
            
            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);

            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            Guid guid = Guid.NewGuid();

            visitorControl.AddVisitors(1);

            Visitor visitor = visitorControl.All().First();

            visitorControl.NotifyOrderReady(guid.ToString());
            Assert.True(visitor.AvailableAt < DateTime.Now && visitor.AvailableAt > DateTime.Now - TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void All_GivenVisitorsInRepo_ExpectRepoCalled()
        {
            repo.Setup(repo => repo.All()).Returns(new List<Visitor>());
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            var visitors = visitorControl.All();
            repo.Verify(repo => repo.All(), Times.Once);
        }

        [Fact]
        public void GetVisitor_GivenVisitorsInRepo_ExpectRepoCalled()
        {
            repo.Setup(repo => repo.GetVisitor(It.IsAny<Guid>())).Returns(new Visitor());
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            var visitor = visitorControl.GetVisitor(Guid.NewGuid());
            repo.Verify(repo => repo.GetVisitor(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public void AddIdleVisitor_GivenVisitorsInRepo_ExpectRepoCalledAndAvailableAtUpdated()
        {
            Visitor visitor = new Visitor();
            DateTime dateTime = DateTime.Now - TimeSpan.FromMinutes(2);
            repo.Setup(repo => repo.GetVisitor(It.IsAny<Guid>())).Returns(visitor);
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.UpdateVisitorAvailabilityAt(Guid.NewGuid(), dateTime);
            repo.Verify(repo => repo.GetVisitor(It.IsAny<Guid>()), Times.Once);
            Assert.Equal(dateTime, visitor.AvailableAt);
        }

        [Fact]
        public void AddIdleVisitor_GivenVisitorIsMissing_ExpectRepoCalledButNothingHappened()
        {
            Visitor visitor = null;
            DateTime dateTime = DateTime.Now - TimeSpan.FromMinutes(2);
            repo.Setup(repo => repo.GetVisitor(It.IsAny<Guid>())).Returns(visitor);
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.UpdateVisitorAvailabilityAt(Guid.NewGuid(), dateTime);
            repo.Verify(repo => repo.GetVisitor(It.IsAny<Guid>()), Times.Once);
            Assert.Null(visitor);
        }

        [Fact]
        public void AddVisitorWaitingForOrder_GivenVisitorWithTicket_ExpectAddedToList()
        {
            Guid visitor = Guid.NewGuid();
            string ticket = "ticket";
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);
            visitorControl.AddVisitorWaitingForOrder(ticket, visitor);

            Assert.Contains(new KeyValuePair<string, Guid>(ticket, visitor),
                visitorControl.VisitorsWaitingForOrder);
            Assert.Single(visitorControl.VisitorsWaitingForOrder);
        }

        [Fact]
        public void AddVisitorWaitingForOrder_GivenVisitorWithTicketAlreadyInList_ExpectNotAddedToList()
        {
            Guid visitor = Guid.NewGuid();
            string ticket = "ticket";
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);
            visitorControl.AddVisitorWaitingForOrder(ticket, visitor);

            Assert.Contains(new KeyValuePair<string, Guid>(ticket, visitor),
                visitorControl.VisitorsWaitingForOrder);
            Assert.Single(visitorControl.VisitorsWaitingForOrder);

            visitorControl.AddVisitorWaitingForOrder(ticket, visitor);

            Assert.Contains(new KeyValuePair<string, Guid>(ticket, visitor),
                visitorControl.VisitorsWaitingForOrder);
            Assert.Single(visitorControl.VisitorsWaitingForOrder);
        }

        [Fact]
        public void HandleIdleVisitors_NoVisitors_ExpectNothingHappened()
        {
            repo.Setup(repo => repo.IdleVisitors()).Returns(new List<Visitor>());
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);
            
            visitorControl.HandleIdleVisitors();
            visitorMovementService.Verify(service => service.SetNextStepDistance(It.IsAny<Visitor>()), Times.Never);
        }
        
        [Fact]
        public void HandleIdleVisitors_VisitorLocationNotSet_ExpectNothingHappened()
        {
            Visitor visitor = new Visitor();

            repo.Setup(repo => repo.IdleVisitors()).Returns(new List<Visitor>(){ visitor});

            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);
                
            VisitorControl visitorControl = new VisitorControl(mediator, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.HandleIdleVisitors();
            visitorMovementService.Verify(service => service.SetNextStepDistance(It.IsAny<Visitor>()), Times.Never);
        }
        
        [Fact]
        public void HandleIdleVisitors_VisitorWithLocation_ExpectLocationNotChangedAndMovedToLocation()
        {
            ILocationDto locationDto = new RideDto();
            Visitor visitor = new Visitor();
            visitor.TargetLocation = locationDto;
            Mock<IMediator> mediator = new Mock<IMediator>();

            repo.Setup(repo => repo.IdleVisitors()).Returns(new List<Visitor>(){ visitor});

            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);
                
            VisitorControl visitorControl = new VisitorControl(mediator.Object, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.HandleIdleVisitors();
            visitorMovementService.Verify(service => service.SetNextStepDistance(It.IsAny<Visitor>()), Times.Once);
            strategyMock.Verify(strategy => strategy.SetNewLocation(It.IsAny<Visitor>()), Times.Never);
            mediator.Verify(mediator => mediator.Publish(It.IsAny<VisitorEvent>(), CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public void HandleIdleVisitors_VisitorWithoutLocation_ExpectLocationAndMovedToLocation()
        {
            ILocationDto locationDto = new RideDto();
            Visitor visitor = new Visitor();
            Mock<IMediator> mediator = new Mock<IMediator>();

            repo.Setup(repo => repo.IdleVisitors()).Returns(new List<Visitor>(){ visitor});

            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);
            strategyMock.Setup(mock => mock.SetNewLocation(visitor)).Callback<Visitor>((visitor) =>
            {
                visitor.TargetLocation = locationDto;
            });
                
            VisitorControl visitorControl = new VisitorControl(mediator.Object, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.HandleIdleVisitors();
            visitorMovementService.Verify(service => service.SetNextStepDistance(It.IsAny<Visitor>()), Times.Once);
            strategyMock.Verify(strategy => strategy.SetNewLocation(It.IsAny<Visitor>()), Times.Once);
            mediator.Verify(mediator => mediator.Publish(It.IsAny<VisitorEvent>(), CancellationToken.None), Times.Once);
        }
        
        [Fact]
        public void HandleIdleVisitors_VisitorInRangeOfLocation_ExpectStrategyCalled()
        {
            ILocationDto locationDto = new RideDto();
            locationDto.Coordinates = new Coordinate(51.6491433, 5.0454834);
            Visitor visitor = new Visitor();
            visitor.CurrentLocation = new Coordinate(51.6491420, 5.0454830);
            visitor.TargetLocation = locationDto;
            visitor.NextStepDistance = 10.0;
            Mock<IMediator> mediator = new Mock<IMediator>();

            repo.Setup(repo => repo.IdleVisitors()).Returns(new List<Visitor>(){ visitor});

            Mock<IVisitorLocationStrategy> strategyMock = new Mock<IVisitorLocationStrategy>();
            locationTypeStrategyMock.Setup(strategyMock => strategyMock.GetStrategy(It.IsAny<LocationType>()))
                .Returns(strategyMock.Object);
            visitor.LocationStrategy = strategyMock.Object;
                
            VisitorControl visitorControl = new VisitorControl(mediator.Object, logger, standClient, repo.Object,
                visitorMovementService.Object, locationTypeStrategyMock.Object);

            visitorControl.HandleIdleVisitors();
            visitorMovementService.Verify(service => service.SetNextStepDistance(It.IsAny<Visitor>()), Times.Once);
            strategyMock.Verify(strategy => strategy.SetNewLocation(It.IsAny<Visitor>()), Times.Never);
            strategyMock.Verify(strategy => strategy.StartLocationActivity(It.IsAny<Visitor>()));
            mediator.Verify(mediator => mediator.Publish(It.IsAny<VisitorEvent>(), CancellationToken.None), Times.Never);
        }
    }
}
