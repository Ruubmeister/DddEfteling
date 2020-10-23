
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.Entrances.Entities;
using DddEfteling.Park.Rides.Boundary;
using DddEfteling.Park.Rides.Controls;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Boundaries
{
    public class RideEntranceEventHandlerTest
    {
        [Fact]
        public void Handle_GivenParkOpened_ExpectCallToOpenRides()
        {
            Mock<IEntranceControl> entranceMock = new Mock<IEntranceControl>();
            Mock<IRideControl> rideMock = new Mock<IRideControl>();
            Mock<ILogger<RideEntranceEventHandler>> loggerMock = new Mock<ILogger<RideEntranceEventHandler>>();

            entranceMock.Setup(control => control.IsOpen()).Returns(true);

            RideEntranceEventHandler eventHandler = new RideEntranceEventHandler(loggerMock.Object, entranceMock.Object, rideMock.Object);

            EntranceEvent entranceEvent = new EntranceEvent(EventType.StatusChanged);

            eventHandler.Handle(entranceEvent, new System.Threading.CancellationToken());

            rideMock.Verify(control => control.CloseRides(), Times.Never());
            rideMock.Verify(control => control.OpenRides(), Times.Once());
        }

        [Fact]
        public void Handle_GivenParkClosed_ExpectCallToCloseRides()
        {
            Mock<IEntranceControl> entranceMock = new Mock<IEntranceControl>();
            Mock<IRideControl> rideMock = new Mock<IRideControl>();
            Mock<ILogger<RideEntranceEventHandler>> loggerMock = new Mock<ILogger<RideEntranceEventHandler>>();

            entranceMock.Setup(control => control.IsOpen()).Returns(false);

            RideEntranceEventHandler eventHandler = new RideEntranceEventHandler(loggerMock.Object, entranceMock.Object, rideMock.Object);

            EntranceEvent entranceEvent = new EntranceEvent(EventType.StatusChanged);

            eventHandler.Handle(entranceEvent, new System.Threading.CancellationToken());

            rideMock.Verify(control => control.OpenRides(), Times.Never());
            rideMock.Verify(control => control.CloseRides(), Times.Once());
        }
    }
}
