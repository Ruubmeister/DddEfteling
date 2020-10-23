using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Boundary;
using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Boundary;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Boundaries
{
    public class FairyTaleBoundaryTest
    {
        IFairyTaleControl fairyTaleControl;
        FairyTaleBoundary fairyTaleBoundary;
        public FairyTaleBoundaryTest()
        {
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            IEventProducer producer = Mock.Of<IEventProducer>();
            IMediator mediator = Mock.Of<IMediator>();
            this.fairyTaleControl = new FairyTaleControl(logger, producer);
            this.fairyTaleBoundary = new FairyTaleBoundary(fairyTaleControl);
        }

        [Fact]
        public void GetFairyTales_RetrieveJson_ExpectsFairyTales()
        {
            ActionResult<List<FairyTaleDto>> tales = fairyTaleBoundary.GetFairyTales();

            Assert.NotEmpty(tales.Value);
        }
    }
}
