using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Stands.Boundaries;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
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
            IRealmControl realmControl = new RealmControl();
            ILogger<IStandControl> logger = Mock.Of<ILogger<IStandControl>>();
            this.standControl = new StandControl(realmControl);
            this.standBoundary = new StandBoundary(standControl);
        }

        [Fact]
        public void GetStands_RetrieveJson_ExpectsStands()
        {
            ActionResult<List<Stand>> stands = standBoundary.GetStands();

            Assert.NotEmpty(stands.Value);
        }
    }
}
