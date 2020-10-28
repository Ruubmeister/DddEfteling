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
        private readonly IFairyTaleClient fairyTaleClient;
        private readonly IRideClient rideClient;
        private readonly IEventProducer eventProducer;

        public VisitorControlTest()
        {
            this.fairyTaleClient = Mock.Of<IFairyTaleClient>();
            this.rideClient = Mock.Of<IRideClient>();
            this.eventProducer = Mock.Of<IEventProducer>();
        }

        [Fact]
        public void AddVisitors_AddThree_ExpectThree()
        {
            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient, fairyTaleClient, eventProducer);

            visitorControl.AddVisitors(3);
            Assert.Equal(3, visitorControl.All().Count);

        }

        [Fact]
        public void NotifyIdleVisitor_GuidGiven_ExpectMediatorCall()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>();

            VisitorControl visitorControl = new VisitorControl(mediatorMock.Object, settings, logger, rideClient, fairyTaleClient, eventProducer);
            Guid guid = Guid.NewGuid();
            visitorControl.NotifyIdleVisitor(guid);

            mediatorMock.Verify(mock => mock.Publish(It.Is<VisitorEvent>(visitorEvent => visitorEvent.VisitorGuid.Equals(guid)), CancellationToken.None));
        }

        [Fact]
        public void HandleBusyVisitors_HasNoBusyVisitors_ExpectNothingHappens()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>();

            VisitorControl visitorControl = new VisitorControl(mediatorMock.Object, settings, logger, rideClient, fairyTaleClient, eventProducer);
            visitorControl.HandleBusyVisitors();

            mediatorMock.Verify(mock => mock.Publish(It.IsAny<VisitorEvent>(), CancellationToken.None), Times.Never);
            Assert.Empty(visitorControl.BusyVisitors);
        }

        [Fact]
        public void HandleBusyVisitors_HasBusyVisitorButNotPassedDate_ExpectNothingHappens()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>();

            VisitorControl visitorControl = new VisitorControl(mediatorMock.Object, settings, logger, rideClient, fairyTaleClient, eventProducer);
            Visitor visitor = new Visitor();
            DateTime dt = DateTime.Now.AddMinutes(1);
            visitorControl.AddBusyVisitor(visitor.Guid, dt);
            visitorControl.HandleBusyVisitors();

            mediatorMock.Verify(mock => mock.Publish(It.IsAny<VisitorEvent>(), CancellationToken.None), Times.Never);
            Assert.Single(visitorControl.BusyVisitors);
        }

        [Fact]
        public void HandleBusyVisitors_HasBusyVisitorAndPassedDate_ExpectCall()
        {
            Mock<IMediator> mediatorMock = new Mock<IMediator>();

            VisitorControl visitorControl = new VisitorControl(mediatorMock.Object, settings, logger, rideClient, fairyTaleClient, eventProducer);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();
            DateTime dt = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));
            visitorControl.AddBusyVisitor(visitor.Guid, dt);
            visitorControl.HandleBusyVisitors();

            Assert.Empty(visitorControl.BusyVisitors);
        }

        [Fact]
        public void AddIdleVisitor_VisitorNotIdleYet_ExpectVisitorInIdleList()
        {
            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient, fairyTaleClient, eventProducer);
            Guid guid = Guid.NewGuid();
            DateTime dt = DateTime.Now;
            visitorControl.AddIdleVisitor(guid, dt);

            Assert.NotEmpty(visitorControl.IdleVisitors);
            Assert.True(visitorControl.IdleVisitors.ContainsKey(guid));
            Assert.Equal(dt, visitorControl.IdleVisitors.First(kv => kv.Key.Equals(guid)).Value);
        }

        [Fact]
        public void AddBusyVisitor_VisitorNotBusyYet_ExpectVisitorInBusyList()
        {
            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient, fairyTaleClient, eventProducer);
            Guid guid = Guid.NewGuid();
            DateTime dt = DateTime.Now;
            visitorControl.AddBusyVisitor(guid, dt);

            Assert.NotEmpty(visitorControl.BusyVisitors);
            Assert.True(visitorControl.BusyVisitors.ContainsKey(guid));
            Assert.Equal(dt, visitorControl.BusyVisitors.First(kv => kv.Key.Equals(guid)).Value);
        }

        [Fact]
        public void GetVisitors_AddThree_ExpectEachVisitor()
        {
            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient, fairyTaleClient, eventProducer);

            visitorControl.AddVisitors(3);
            List<Visitor> visitors = visitorControl.All();
            visitors.ForEach(visitor => Assert.Equal(visitor, visitorControl.GetVisitor(visitor.Guid)));
        }

        [Fact]
        public void SetNewLocation_GetLocationWithoutPreviousType_ExpectRideStandOrFairyTale()
        {

            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());


            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            Visitor visitor = new Visitor();

            Assert.Null(visitor.TargetLocation);

            visitorControl.SetNewLocation(visitor);
            Assert.NotNull(visitor.TargetLocation);
        }

        [Fact]
        public void SetNewLocation_GetLocationWithPreviousTypeIsRide_ExpectRide()
        {
            RideDto rideDto = new RideDto();
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            Visitor visitor = new Visitor();
            visitor.AddVisitedLocation(rideDto);

            Assert.Null(visitor.TargetLocation);

            visitorControl.SetNewLocation(visitor);
            Assert.NotNull(visitor.TargetLocation);
            Assert.Equal(LocationType.RIDE, visitor.TargetLocation.LocationType);
        }

        [Fact]
        public void SetNewLocation_GetLocationWithPreviousTypeIsFairyTale_ExpectFairyTale()
        {
            FairyTaleDto fairyTaleDto = new FairyTaleDto();

            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            Visitor visitor = new Visitor();
            visitor.AddVisitedLocation(fairyTaleDto);

            Assert.Null(visitor.TargetLocation);

            visitorControl.SetNewLocation(visitor);
            Assert.NotNull(visitor.TargetLocation);
            Assert.Equal(LocationType.FAIRYTALE, visitor.TargetLocation.LocationType);
        }


        [Fact]
        public void SetNewLocation_GetLocationWithPreviousTypeIsStand_ExpectStand()
        {
            StandDto standDto = new StandDto();

            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(new FairyTaleDto());
            rideClient.Setup(client => client.GetRandomRide()).Returns(new RideDto());

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            Visitor visitor = new Visitor();
            visitor.AddVisitedLocation(standDto);

            Assert.Null(visitor.TargetLocation);

            visitorControl.SetNewLocation(visitor);
            Assert.NotNull(visitor.TargetLocation);
            Assert.Equal(LocationType.RIDE, visitor.TargetLocation.LocationType);
        }

        [Fact]
        public void HandleIdleVisitors_HasIdleVisitorWithoutLocation_ExpectLocationAndStep()
        {
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            FairyTaleDto taleDto = new FairyTaleDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            RideDto rideDto = new RideDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(taleDto);
            rideClient.Setup(client => client.GetRandomRide()).Returns(rideDto);

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();
            visitor.TargetLocation = null;

            DateTime idleDt = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            visitorControl.AddIdleVisitor(visitor.Guid, idleDt);
            Assert.Null(visitor.TargetLocation);
            Assert.True(visitor.CurrentLocation.Latitude > 0);
            Assert.True(visitor.CurrentLocation.Longitude > 0);
            Coordinate startLocation = visitor.CurrentLocation;

            Assert.NotEmpty(visitorControl.IdleVisitors);
            visitorControl.HandleIdleVisitors();
            Assert.Empty(visitorControl.IdleVisitors);
            Assert.True(visitor.CurrentLocation.Latitude > startLocation.Latitude);
            Assert.True(visitor.CurrentLocation.Longitude > startLocation.Longitude);
            Assert.True(visitor.CurrentLocation.Latitude < 51.65224);
            Assert.True(visitor.CurrentLocation.Longitude < 5.05222);
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public void HandleIdleVisitors_HasNoIdleVisitorWithoutLocation_ExpectNothingHappens()
        {
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            FairyTaleDto taleDto = new FairyTaleDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            RideDto rideDto = new RideDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(taleDto);
            rideClient.Setup(client => client.GetRandomRide()).Returns(rideDto);

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();
            Coordinate startLocation = visitor.CurrentLocation;

            Assert.Empty(visitorControl.IdleVisitors);
            visitorControl.HandleIdleVisitors();
            Assert.Empty(visitorControl.IdleVisitors);
            Assert.Equal(startLocation.Latitude, visitor.CurrentLocation.Latitude);
            Assert.Equal(startLocation.Longitude, visitor.CurrentLocation.Longitude);
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public void HandleIdleVisitors_HasIdleVisitorWithLocation_ExpectSameLocationAndStep()
        {
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            FairyTaleDto taleDto = new FairyTaleDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            RideDto rideDto = new RideDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(taleDto);
            rideClient.Setup(client => client.GetRandomRide()).Returns(rideDto);

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();

            DateTime idleDt = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            visitorControl.AddIdleVisitor(visitor.Guid, idleDt);
            Assert.NotNull(visitor.TargetLocation);
            Assert.True(visitor.CurrentLocation.Latitude > 0);
            Assert.True(visitor.CurrentLocation.Longitude > 0);
            Coordinate startLocation = visitor.CurrentLocation;

            Assert.NotEmpty(visitorControl.IdleVisitors);
            visitorControl.HandleIdleVisitors();
            Assert.Empty(visitorControl.IdleVisitors);
            Assert.True(visitor.CurrentLocation.Latitude > startLocation.Latitude);
            Assert.True(visitor.CurrentLocation.Longitude > startLocation.Longitude);
            Assert.True(visitor.CurrentLocation.Latitude < 51.65224);
            Assert.True(visitor.CurrentLocation.Longitude < 5.05222);
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public void HandleIdleVisitors_HasIdleVisitorWithLocationIsRideInRange_ExpectArriveAtRide()
        {
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            FairyTaleDto taleDto = new FairyTaleDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            RideDto rideDto = new RideDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(taleDto);
            rideClient.Setup(client => client.GetRandomRide()).Returns(rideDto);

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();
            visitor.TargetLocation = rideDto;
            visitor.CurrentLocation = new Coordinate(51.65223999999, 5.0522199999);

            DateTime idleDt = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            visitorControl.AddIdleVisitor(visitor.Guid, idleDt);
            Assert.NotNull(visitor.TargetLocation);
            Assert.True(visitor.CurrentLocation.Latitude > 0);
            Assert.True(visitor.CurrentLocation.Longitude > 0);
            Coordinate startLocation = visitor.CurrentLocation;

            Assert.NotEmpty(visitorControl.IdleVisitors);
            visitorControl.HandleIdleVisitors();
            Assert.Empty(visitorControl.IdleVisitors);
            eventProducer.Verify(producer => producer.Produce(It.Is<Event>(eventOut => eventOut.Type.Equals(EventType.StepInRideLine))));
        }

        [Fact]
        public void HandleIdleVisitors_HasIdleVisitorWithLocationIsFairyTaleInRange_ExpectArriveAtRide()
        {
            Mock<IFairyTaleClient> fairyTaleClient = new Mock<IFairyTaleClient>();
            Mock<IRideClient> rideClient = new Mock<IRideClient>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();

            FairyTaleDto taleDto = new FairyTaleDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            RideDto rideDto = new RideDto
            {
                Coordinates = new Coordinate(51.65224, 5.05222)
            };

            fairyTaleClient.Setup(client => client.GetRandomFairyTale()).Returns(taleDto);
            rideClient.Setup(client => client.GetRandomRide()).Returns(rideDto);

            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, rideClient.Object, fairyTaleClient.Object, eventProducer.Object);
            visitorControl.AddVisitors(1);
            Visitor visitor = visitorControl.All().First();
            visitor.TargetLocation = taleDto;
            visitor.CurrentLocation = new Coordinate(51.65223999999, 5.0522199999);

            DateTime idleDt = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));

            visitorControl.AddIdleVisitor(visitor.Guid, idleDt);
            Assert.NotNull(visitor.TargetLocation);
            Assert.True(visitor.CurrentLocation.Latitude > 0);
            Assert.True(visitor.CurrentLocation.Longitude > 0);
            Coordinate startLocation = visitor.CurrentLocation;

            Assert.NotEmpty(visitorControl.IdleVisitors);
            visitorControl.HandleIdleVisitors();
            Assert.Empty(visitorControl.IdleVisitors);
            eventProducer.Verify(producer => producer.Produce(It.Is<Event>(eventOut => eventOut.Type.Equals(EventType.ArrivedAtFairyTale))));
        }

    }
}
