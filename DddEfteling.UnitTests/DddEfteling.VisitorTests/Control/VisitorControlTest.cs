using DddEfteling.Shared.Boundary;
using DddEfteling.Visitors.Boundary;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DddEfteling.Tests.Visitors.Control
{
    public class VisitorControlTest
    {

        IMediator mediator = new Mock<IMediator>().Object;
        ILogger<VisitorControl> logger = new Mock<ILogger<VisitorControl>>().Object;
        IOptions<VisitorSettings> settings = new Mock<IOptions<VisitorSettings>>().Object;
        IFairyTaleClient fairyTaleClient;
        IRideClient rideClient;
        IEventProducer eventProducer;

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
    }
}
