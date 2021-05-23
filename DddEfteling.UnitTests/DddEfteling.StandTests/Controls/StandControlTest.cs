using System;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Controls;
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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            this.standControl = new StandControl(logger, eventProducer, locationService);
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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);
            
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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);
            
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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);
            
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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);

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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);

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
            ILocationService locationService = new LocationService(Mock.Of<ILogger<LocationService>>(), 
                new Random());
            
            StandControl standControl = new StandControl(logger, eventProducer.Object, openDinnerOrders, ordersDoneAtTime, locationService);

            Dinner result = standControl.GetReadyDinner(dinnerGuid.ToString());
            Assert.Equal(dinner, result);
            
            // It should now throw a null exception, as the dinner is removed
            Assert.Throws<ArgumentNullException>(() => standControl.GetReadyDinner(dinnerGuid.ToString()));
        }
    }
}
