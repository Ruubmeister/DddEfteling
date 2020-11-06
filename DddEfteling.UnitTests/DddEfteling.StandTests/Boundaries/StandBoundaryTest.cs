using DddEfteling.Shared.Boundaries;
using DddEfteling.Stands.Boundaries;
using DddEfteling.Stands.Controls;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.StandTests.Boundaries
{
    public class RideBoundaryTest
    {
        private readonly IStandControl standControl;
        private readonly StandBoundary standBoundary;
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
