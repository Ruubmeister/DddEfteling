using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Controls;
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
            this.eventProducer = new Mock<IEventProducer>();
            this.fairyTaleControl = new FairyTaleControl(logger, this.eventProducer.Object);

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
        public void NearestFairyTale_GetNearestFromDeZesDienarenWithoutExclusions_ExpectKleineZeemeermin()
        {
            FairyTale tale = this.fairyTaleControl.All().First(tale => tale.Name.Equals("De Zes Dienaren"));

            FairyTale closest = this.fairyTaleControl.NearestFairyTale(tale.Guid, new List<System.Guid>());
            Assert.NotNull(closest);
            Assert.Equal("De Kleine Zeemeermin", closest.Name);
        }

        [Fact]
        public void NearestFairyTale_GetNearestFromDeZesDienarenWithExclusions_ExpectKleineZeemeermin()
        {
            FairyTale tale = this.fairyTaleControl.All().First(tale => tale.Name.Equals("De Zes Dienaren"));

            List<FairyTale> excludedTales = this.fairyTaleControl.All().Where(tale => tale.Name.Equals("De Kleine Zeemeermin") || tale.Equals("Raponsje") || tale.Equals("Roodkapje")).ToList();

            FairyTale closest = this.fairyTaleControl.NearestFairyTale(tale.Guid, excludedTales.ConvertAll(tale => tale.Guid));
            Assert.NotNull(closest);
            Assert.Equal("Draak Lichtgeraakt", closest.Name);
        }

        [Fact]
        public void NextFairyTale_GetNearestFromDeZesDienarenWithoutExclusions_ExpectCorrectTale()
        {
            FairyTale tale = this.fairyTaleControl.All().First(tale => tale.Name.Equals("De Zes Dienaren"));

            FairyTale closest = this.fairyTaleControl.NextLocation(tale.Guid, new List<System.Guid>());
            Assert.NotNull(closest);

            List<string> expected = new List<string>() { "Draak Lichtgeraakt", "De Kleine Zeemeermin", "Roodkapje" };
            Assert.Contains(closest.Name, expected);
        }

        [Fact]
        public void NextFairyTale_GetNearestFromDeZesDienarenWithExclusions_ExpectCorrectTale()
        {
            FairyTale tale = this.fairyTaleControl.All().First(tale => tale.Name.Equals("De Zes Dienaren"));

            List<FairyTale> excludedTales = this.fairyTaleControl.All().Where(tale => tale.Name.Equals("De Kleine Zeemeermin") || tale.Name.Equals("Raponsje") || tale.Name.Equals("Roodkapje")).ToList();

            FairyTale closest = this.fairyTaleControl.NextLocation(tale.Guid, excludedTales.ConvertAll(tale => tale.Guid));
            Assert.NotNull(closest);

            List<string> expected = new List<string>() { "Draak Lichtgeraakt", "Kabouterdorp", "Het stoute prinsesje" };
            Assert.Contains(closest.Name, expected);
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
