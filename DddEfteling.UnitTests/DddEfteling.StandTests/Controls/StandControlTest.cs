using DddEfteling.Stands.Controls;
using DddEfteling.Stands.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Controls
{
    public class StandControlTest
    {
        IStandControl standControl;
        public StandControlTest()
        {
            this.standControl = new StandControl();
        }

        [Fact]
        public void FindRideByName_FindPollesPannenkoeken_ExpectStand()
        {

            Stand stand = standControl.FindStandByName("Polles pannenkoeken");
            Assert.NotNull(stand);
            Assert.Equal("Polles pannenkoeken", stand.Name);
        }

        [Fact]
        public void All_GetAllStands_ExpectStands()
        {
            List<Stand> stands = standControl.All();
            Assert.NotEmpty(stands);
            Assert.Single(stands.Where(stand => stand.Name.Equals("Polles pannenkoeken")));
        }
    }
}
