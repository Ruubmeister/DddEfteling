using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Controls;
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Boundaries;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.FairyTaleTests.Boundaries
{
    public class FairyTaleBoundaryTest
    {
        private readonly Mock<IFairyTaleControl> fairyTaleControl;
        private readonly FairyTaleBoundary fairyTaleBoundary;
        public FairyTaleBoundaryTest()
        {
            this.fairyTaleControl = new Mock<IFairyTaleControl>();
            this.fairyTaleBoundary = new FairyTaleBoundary(fairyTaleControl.Object);
        }

        [Fact]
        public void GetFairyTales_RetrieveJson_ExpectsFairyTales()
        {
            this.fairyTaleControl.Setup(control => control.All()).Returns(new List<FairyTale>() { { new FairyTale() }, { new FairyTale() } });
            ActionResult<List<FairyTaleDto>> tales = fairyTaleBoundary.GetFairyTales();

            Assert.NotEmpty(tales.Value);
        }

        [Fact]
        public void GetRandomFairyTale_HasFairyTales_ExpectFairyTale()
        {
            this.fairyTaleControl.Setup(control => control.GetRandom()).Returns(new FairyTale());
            ActionResult<FairyTaleDto> tale = fairyTaleBoundary.GetRandomFairyTale();

            Assert.NotNull(tale);
        }

        [Fact]
        public void GetNewFairyTaleLocation_HasFairyTales_ExpectFairyTale()
        {
            this.fairyTaleControl.Setup(control => control.NextLocation(It.IsAny<Guid>(), It.IsAny<List<Guid>>())).Returns(new FairyTale());
            ActionResult<FairyTaleDto> tale = fairyTaleBoundary.GetNewFairyTaleLocation(Guid.NewGuid(), "");

            Assert.NotNull(tale);
        }
    }
}
