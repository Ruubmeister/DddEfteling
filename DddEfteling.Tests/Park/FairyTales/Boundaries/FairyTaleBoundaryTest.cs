using DddEfteling.Park.FairyTales.Boundaries;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
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
            IRealmControl realmControl = new RealmControl();
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            IMediator mediator = Mock.Of<IMediator>();
            this.fairyTaleControl = new FairyTaleControl(realmControl, logger, mediator);
            this.fairyTaleBoundary = new FairyTaleBoundary(fairyTaleControl);
        }

        [Fact]
        public void GetFairyTales_RetrieveJson_ExpectsFairyTales()
        {
            ActionResult<List<FairyTale>> tales = fairyTaleBoundary.GetFairyTales();

            Assert.NotEmpty(tales.Value);
        }
    }
}
