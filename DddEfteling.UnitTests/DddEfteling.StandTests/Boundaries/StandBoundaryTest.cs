using DddEfteling.Shared.Boundaries;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.StandTests.Boundaries
{
    public class StandBoundaryTest
    {
        private readonly IStandControl standControl;
        private readonly StandBoundary standBoundary;
        public StandBoundaryTest()
        {
            ILogger<StandControl> logger = Mock.Of<ILogger<StandControl>>();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            this.standControl = new StandControl(logger, eventProducer);
            this.standBoundary = new StandBoundary(standControl);
        }

        [Fact]
        public void GetStands_RetrieveJson_ExpectsStands()
        {
            ActionResult<List<StandDto>> stands = standBoundary.GetStands();

            Assert.NotEmpty(stands.Value);
        }

        [Fact]
        public void GetStand_ContainsStand_ExpectStand()
        {
            Stand randomStand = standControl.GetRandom();
            ActionResult<StandDto> stand = standBoundary.GetStand(randomStand.Guid);

            Assert.Equal(randomStand.Name, stand.Value.Name);
            Assert.Equal(randomStand.Guid, stand.Value.Guid);
        }

        [Fact]
        public void GetStand_StandIsMissing_ExpectNotFound()
        {
            Guid guid = Guid.NewGuid();
            ActionResult<StandDto> stand = standBoundary.GetStand(guid);

            Assert.Null(stand.Value);
            Assert.IsAssignableFrom<NotFoundResult>(stand.Result);
        }

        [Fact]
        public void GetRandomStand_ContainsStands_ExpectStand()
        {
            Stand randomStand = standControl.GetRandom();

            Assert.NotNull(randomStand);

            Assert.Contains(randomStand, standControl.All());
        }

        [Fact]
        public void OrderDinner_StandDoesNotExist_ExpectNotFound()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetStand(It.IsAny<Guid>())).Throws(new ArgumentNullException());
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<string> ticket = standBoundary.OrderDinner(Guid.NewGuid(), new List<string>());

            Assert.IsAssignableFrom<NotFoundResult>(ticket.Result);
        }

        [Fact]
        public void OrderDinner_StandExists_ExpectOrderPlaced()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetStand(It.IsAny<Guid>())).Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            standControl.Setup(control => control.PlaceOrder(It.IsAny<Guid>(), It.IsAny<List<String>>())).Returns(Guid.NewGuid().ToString);
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<string> ticket = standBoundary.OrderDinner(Guid.NewGuid(), new List<string>());

            Assert.IsAssignableFrom<OkObjectResult>(ticket.Result);
            Assert.NotNull(ticket.Result);
        }

        [Fact]
        public void OrderDinner_OrderTicketMissing_ExpectBadRequest()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetStand(It.IsAny<Guid>())).Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            standControl.Setup(control => control.PlaceOrder(It.IsAny<Guid>(), It.IsAny<List<String>>())).Returns((string) null);
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<string> ticket = standBoundary.OrderDinner(Guid.NewGuid(), new List<string>());

            Assert.IsAssignableFrom<BadRequestResult>(ticket.Result);
        }
        
        [Fact]
        public void OrderDinner_InvalidOperationException_ExpectsNoContent()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetStand(It.IsAny<Guid>())).Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            standControl.Setup(control => control.PlaceOrder(It.IsAny<Guid>(), It.IsAny<List<String>>())).Throws(
                new InvalidOperationException());
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<string> ticket = standBoundary.OrderDinner(Guid.NewGuid(), new List<string>());

            Assert.IsAssignableFrom<NotFoundResult>(ticket.Result);
        } 

        [Fact]
        public void GetOrder_DinnerIsNull_ExpectBadRequest()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetReadyDinner(It.IsAny<string>())).Returns((Dinner)null);
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<DinnerDto> dinner = standBoundary.GetOrder("Ticket");

            Assert.IsAssignableFrom<BadRequestResult>(dinner.Result);
        }

        [Fact]
        public void GetOrder_AllOk_ExpectDinnerDto()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetReadyDinner(It.IsAny<string>())).Returns(new Dinner());
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<DinnerDto> dinner = standBoundary.GetOrder("Ticket");

            Assert.IsAssignableFrom<OkObjectResult>(dinner.Result);
        }

        [Fact]
        public void GetOrder_GetsInvalidOperationException_ExpectNoContent()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetReadyDinner(It.IsAny<string>())).Throws<InvalidOperationException>();
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<DinnerDto> dinner = standBoundary.GetOrder("Ticket");

            Assert.IsAssignableFrom<NoContentResult>(dinner.Result);
        }

        [Fact]
        public void GetOrder_GetsArgumentNullException_ExpectBadRequest()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetReadyDinner(It.IsAny<string>())).Throws<ArgumentNullException>();
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<DinnerDto> dinner = standBoundary.GetOrder("Ticket");

            Assert.IsAssignableFrom<BadRequestResult>(dinner.Result);
        }

        [Fact]
        public void GetNewStandLocation_GivenStand_ExpectNewStand()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.NextLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<StandDto> stand = standBoundary.GetNewStandLocation(Guid.NewGuid(), "");
            Assert.IsAssignableFrom<OkObjectResult>(stand.Result);
        }

        [Fact]
        public void GetNewStandLocation_GivenStandWithExclusions_ExpectNewStand()
        {
            string exclusionList = String.Format("{0},{1}", Guid.NewGuid(), Guid.NewGuid());

            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.NextLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            StandBoundary standBoundary = new StandBoundary(standControl.Object);

            ActionResult<StandDto> stand = standBoundary.GetNewStandLocation(Guid.NewGuid(), exclusionList);
            Assert.IsAssignableFrom<OkObjectResult>(stand.Result);
        }

        [Fact]
        public void GetRandomStand_GivenStandExist_ExpectRandomStand()
        {
            Mock<IStandControl> standControl = new Mock<IStandControl>();
            standControl.Setup(control => control.GetRandom())
                .Returns(new Stand("Stand", new Geolocation.Coordinate(), new List<Product>()));
            StandBoundary standBoundary = new StandBoundary(standControl.Object);
            ActionResult<StandDto> stand = standBoundary.GetRandomStand();
            
            Assert.IsAssignableFrom<OkObjectResult>(stand.Result);
        }
        
        
    }
}
