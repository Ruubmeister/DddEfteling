using DddEfteling.Shared.Boundary;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Boundaries
{
    public class RideBoundaryTest
    {
        IStandControl standControl;
        StandBoundary standBoundary;
        public RideBoundaryTest()
        {
            ILogger<IStandControl> logger = Mock.Of<ILogger<IStandControl>>();
            this.standControl = new StandControl();
            this.standBoundary = new StandBoundary(standControl);
        }

        [Fact]
        public void GetStands_RetrieveJson_ExpectsStands()
        {
            ActionResult<List<StandDto>> stands = standBoundary.GetStands();

            Assert.NotEmpty(stands.Value);
        }
    }
}
