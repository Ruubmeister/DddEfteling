using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Controls;
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Controls;
using Xunit;

namespace DddEfteling.FairyTaleTests.Controls
{
    public class FairyTaleControlTest
    {
        private readonly IFairyTaleControl fairyTaleControl;
        private readonly Mock<IEventProducer> eventProducer;

        public FairyTaleControlTest()
        {
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            ILogger<LocationService> locationLogger = Mock.Of<ILogger<LocationService>>();
            this.eventProducer = new Mock<IEventProducer>();
            ILocationService locationService = new LocationService(locationLogger, new Random());
            this.fairyTaleControl = new FairyTaleControl(logger, this.eventProducer.Object, locationService);

        }

        [Fact]
        public void HandleVisitorArrivingAtFairyTale_GivenVisitorArriving_ExpectEventProducerCalled()
        {
            Guid guid = Guid.NewGuid();
            fairyTaleControl.HandleVisitorArrivingAtFairyTale(guid);

            eventProducer.Verify(eventProducer => eventProducer.Produce(It.IsAny<Event>()));
        }

        /*        [Fact]
                public void NotifyForIdleVisitors_GetTwoIdleVisitors_ExpectTwoNotifications()
                {
                    VisitorSettings settings = new VisitorSettings();
                    settings.FairyTaleMaxVisitingSeconds = 2;
                    settings.FairyTaleMinVisitingSeconds = 1;
                    Mock<IOptions<VisitorSettings>> visitorSettingsMock = new Mock<IOptions<VisitorSettings>>();
                    visitorSettingsMock.Setup(settings => settings.Value).Returns(settings);

                    Visitor visitor1 = new Visitor(System.DateTime.Now, 1.22, new Coordinate(), new Random(), visitorSettingsMock.Object);
                    FairyTale tale = fairyTaleControl.GetRandom();
                    visitor1.WatchFairyTale(tale);
                    fairyTaleControl.NotifyForIdleVisitors();
                    this.mediatorMock.Verify(mediator => mediator.Publish(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Never());
                    Task.Delay(2000).Wait();
                    fairyTaleControl.NotifyForIdleVisitors();
                    this.mediatorMock.Verify(mediator => mediator.Publish(It.IsAny<Event>(), It.IsAny<CancellationToken>()), Times.Once());
                }*/
    }
}
