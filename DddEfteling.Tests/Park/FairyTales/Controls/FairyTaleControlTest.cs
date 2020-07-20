using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Visitors.Controls;
using DddEfteling.Park.Visitors.Entities;
using DddEfteling.Tests.Park.FairyTales.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Controls
{
    public class FairyTaleControlTest
    {
        IFairyTaleControl fairyTaleControl;
        Mock<IMediator> mediatorMock;

        public FairyTaleControlTest()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            this.mediatorMock = new Mock<IMediator>();
            this.fairyTaleControl = new FairyTaleControl(realmControl, logger, this.mediatorMock.Object);

        }

        [Fact]
        public void FindFairyTaleByName_FindSneeuwwitje_ExpectFairyTale()
        {
           
            FairyTale tale = fairyTaleControl.FindFairyTaleByName("Sneeuwwitje");
            Assert.NotNull(tale);
            Assert.Equal("Sneeuwwitje", tale.Name);
        }

        [Fact]
        public void All_GetAllFairyTales_ExpectAllFairytales()
        {
            List<FairyTale> fairyTales = fairyTaleControl.All();

            Assert.NotEmpty(fairyTales);
            Assert.Single(fairyTales.Where(fairytale => fairytale.Name.Equals("Sneeuwwitje")));
        }

        [Fact]
        public void Random_GetRandomTale_ExpectRandomTale()
        {
            FairyTale tale = fairyTaleControl.GetRandom();
            Assert.NotNull(tale);
            Assert.Contains(tale, fairyTaleControl.All());
        }

        [Fact]
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
        }
    }
}
