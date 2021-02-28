using System;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Entities;
using Xunit;

namespace DddEfteling.StandTests.Controls
{
    public class StandControlTest
    {
        private readonly IStandControl standControl;
        public StandControlTest()
        {
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            this.standControl = new StandControl(logger, eventProducer);
        }

        [Fact]
        public void FindRideByName_FindPollesPannenkoeken_ExpectStand()
        {

            Stand stand = standControl.FindStandByName("Polles pannenkoeken");
            Assert.NotNull(stand);
            Assert.Equal("Polles pannenkoeken", stand.Name);
        }

        [Fact]
        public void All_GetAllStands_ExpectStands()
        {
            List<Stand> stands = standControl.All();
            Assert.NotEmpty(stands);
            Assert.Single(stands.Where(stand => stand.Name.Equals("Polles pannenkoeken")));
        }

        [Fact]
        public void PlaceOrder_GivenValidOrder_ExpectTicket()
        {
            var stand = standControl.GetRandom();
            var products = new List<string>()
            {
                stand.Meals.First().Name,
                stand.Drinks.First().Name
            };

            var ticket = standControl.PlaceOrder(stand.Guid, products);
            
            Assert.NotEmpty(ticket);
        }
        
        [Fact]
        public void PlaceOrder_GivenNoProducts_ExpectInvalidOperationException()
        {
            var stand = standControl.GetRandom();
            var products = new List<string>();

            Assert.Throws<InvalidOperationException>(() => standControl.PlaceOrder(stand.Guid, products));
        }

        [Fact]
        public void HandleProducedOrders_NoOrders_ExpectNothingHappens()
        {
            var openDinnerOrders = new Dictionary<Guid, Dinner>();
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>();
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);
            
            standControl.HandleProducedOrders();
            
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);
        }
        
        [Fact]
        public void HandleProducedOrders_NoOrdersDone_ExpectNothingHappens()
        {
            Dinner dinner1 = new Dinner();
            Dinner dinner2 = new Dinner();
            Guid dinner1Guid = Guid.NewGuid();
            Guid dinner2Guid = Guid.NewGuid();
            
            var openDinnerOrders = new Dictionary<Guid, Dinner>()
            {
                {dinner1Guid, dinner1},
                {dinner2Guid, dinner2}
            };
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>()
            {
                {dinner1Guid, DateTime.Now.AddSeconds(60)},
                {dinner2Guid, DateTime.Now.AddSeconds(60)}
            };
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);
            
            standControl.HandleProducedOrders();
            
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Never);
        }
        
        [Fact]
        public void HandleProducedOrders_OneOrderDone_ExpectProducerCalledOnce()
        {
            Dinner dinner1 = new Dinner();
            Dinner dinner2 = new Dinner();
            Guid dinner1Guid = Guid.NewGuid();
            Guid dinner2Guid = Guid.NewGuid();
            
            var openDinnerOrders = new Dictionary<Guid, Dinner>()
            {
                {dinner1Guid, dinner1},
                {dinner2Guid, dinner2}
            };
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>()
            {
                {dinner1Guid, DateTime.Now},
                {dinner2Guid, DateTime.Now.AddSeconds(60)}
            };
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);
            
            standControl.HandleProducedOrders();
            
            eventProducer.Verify(producer => producer.Produce(It.IsAny<Event>()), Times.Once);
        }
        
        [Fact]
        public void GetReadyDinner_OrderNotDone_ExpectInvalidOperationException()
        {
            Dinner dinner1 = new Dinner();
            Guid dinner1Guid = Guid.NewGuid();
            
            var openDinnerOrders = new Dictionary<Guid, Dinner>()
            {
                {dinner1Guid, dinner1}
            };
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>()
            {
                {dinner1Guid, DateTime.Now.AddSeconds(60)}
            };
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);

            Assert.Throws<InvalidOperationException>(() => standControl.GetReadyDinner(dinner1Guid.ToString()));
        }
        
        [Fact]
        public void GetReadyDinner_OrderMissing_ExpectArgumentNullException()
        {
            Guid dinnerGuid = Guid.NewGuid();

            var openDinnerOrders = new Dictionary<Guid, Dinner>();
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>();
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);

            Assert.Throws<ArgumentNullException>(() => standControl.GetReadyDinner(dinnerGuid.ToString()));
        }
        
        [Fact]
        public void GetReadyDinner_OrderDone_ExpectDinnerAndRemovedFromList()
        {
            Dinner dinner = new Dinner();
            Guid dinnerGuid = Guid.NewGuid();
            
            var openDinnerOrders = new Dictionary<Guid, Dinner>()
            {
                {dinnerGuid, dinner}
            };
            var ordersDoneAtTime = new Dictionary<Guid, DateTime>();
            
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            Mock<IEventProducer> eventProducer = new Mock<IEventProducer>();
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime);

            Dinner result = standControl.GetReadyDinner(dinnerGuid.ToString());
            Assert.Equal(dinner, result);
            
            // It should now throw a null exception, as the dinner is removed
            Assert.Throws<ArgumentNullException>(() => standControl.GetReadyDinner(dinnerGuid.ToString()));
        }
        
        [Fact]
        public void NearestStand_GetNearestFromDeGuldenGaardeWithoutExclusions_ExpectLekkernijen()
        {
            Stand stand = this.standControl.All().First(tale => tale.Name.Equals("De Gulden Gaarde"));

            Stand closest = this.standControl.NearestStand(stand.Guid, new List<System.Guid>());
            Assert.NotNull(closest);
            Assert.Equal("Lekkernijen", closest.Name);
        }

        [Fact]
        public void NearestFairyTale_GetNearestFromDeGuldenGaardeWithExclusions_ExpectKleineZeemeermin()
        {
            Stand stand = this.standControl.All().First(currStand => currStand.Name.Equals("De Gulden Gaarde"));

            List<Stand> excludedStands = this.standControl.All().Where(currStand => currStand.Name.Equals("Lekkernijen")).ToList();

            Stand closest = this.standControl.NearestStand(stand.Guid, excludedStands.ConvertAll(tale => tale.Guid));
            Assert.NotNull(closest);
            Assert.Equal("Unoxkraam Marerijk", closest.Name);
        }

        [Fact]
        public void NextFairyTale_GetNearestFromDeGuldenGaardeWithoutExclusions_ExpectCorrectTale()
        {
            Stand stand = this.standControl.All().First(currStand => currStand.Name.Equals("De Gulden Gaarde"));

            Stand closest = this.standControl.NextLocation(stand.Guid, new List<System.Guid>());
            Assert.NotNull(closest);

            List<string> expected = new List<string>() { "Unoxkraam Marerijk", "Lekkernijen", "Eigenheymer Marerijk" };
            Assert.Contains(closest.Name, expected);
        }

        [Fact]
        public void NextFairyTale_GetNearestFromDeZesDienarenWithExclusions_ExpectCorrectTale()
        {
            Stand stand = this.standControl.All().First(currStand => currStand.Name.Equals("De Gulden Gaarde"));

            List<Stand> excluded = this.standControl.All().Where(currStand => currStand.Name.Equals("Lekkernijen") || currStand.Name.Equals("Unoxkraam Marerijk")).ToList();

            Stand closest = this.standControl.NextLocation(stand.Guid, excluded.ConvertAll(currStand => currStand.Guid));
            Assert.NotNull(closest);

            List<string> expected = new List<string>() { "Eigenheymer Marerijk", "De Kleyne Klaroen", "Het Wapen van Raveleijn" };
            Assert.Contains(closest.Name, expected);
        }
    }
}
