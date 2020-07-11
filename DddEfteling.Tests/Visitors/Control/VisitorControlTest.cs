using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Visitors.Controls;
using DddEfteling.Park.Visitors.Entities;
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
        FairyTaleControl fairyTaleControl;
        RideControl rideControl;
        StandControl standControl;

        public VisitorControlTest()
        {
            Mock<FairyTaleControl> fairyTaleMock = new Mock<FairyTaleControl>();
            Mock<RideControl> rideMock = new Mock<RideControl>();
            Mock<StandControl> standMock = new Mock<StandControl>();

            this.fairyTaleControl = fairyTaleMock.Object;
            this.rideControl = rideMock.Object;
            this.standControl = standMock.Object;
        }

        [Fact]
        public void AddVisitors_AddThree_ExpectThree()
        {
            VisitorControl visitorControl = new VisitorControl(mediator, settings, logger, fairyTaleControl, rideControl, standControl);

            visitorControl.AddVisitors(3);
            Assert.Equal(3, visitorControl.All().Count);

        }
    }
}
