
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace DddEfteling.Tests.Visitors.Entities
{
    public class VisitorTest
    {

        Coordinate startCoordinate = new Coordinate(1.0, 1.0);
        Random random = new Random();
        IMediator mediator = new Mock<IMediator>().Object;
        IOptions<VisitorSettings> settings = new Mock<IOptions<VisitorSettings>>().Object;

        public VisitorTest()
        {
        }

        [Fact]
        public void Construct_createVisitor_expectVisitor()
        {
            DateTime dateOfBirth = DateTime.Now;
            dateOfBirth.Subtract(TimeSpan.FromDays(365*20));
            Visitor visitor = new Visitor(dateOfBirth, 1.73, startCoordinate, random, settings);

            Assert.Equal(dateOfBirth, visitor.DateOfBirth );
            Assert.False(visitor.Guid == Guid.Empty);
            Assert.Equal(1.73, visitor.Length);
        }

        [Fact]
        public void WatchFairyTale_LetVisitorWatchFairyTale_ExpectTookTime()
        {
            DateTime start = DateTime.Now;
            Mock<IOptions<VisitorSettings>> settingsMock = new Mock<IOptions<VisitorSettings>>();
            VisitorSettings visitorSettings = new VisitorSettings();
            visitorSettings.FairyTaleMinVisitingSeconds = 3;
            visitorSettings.FairyTaleMaxVisitingSeconds = 5;
            settingsMock.Setup(setting => setting.Value).Returns(visitorSettings);

            Visitor visitor = new Visitor(start, 1.73, startCoordinate, random, settingsMock.Object);

            visitor.WatchFairyTale(new FairyTale());
            DateTime end = DateTime.Now;

            TimeSpan duration = end - start;

            Assert.True(duration.TotalSeconds >= 3);

            Assert.True(duration.TotalSeconds <= 5);

        }
    }
}
