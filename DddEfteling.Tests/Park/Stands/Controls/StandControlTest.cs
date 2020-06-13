using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
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

            IRealmControl realmControl = new RealmControl();
            ILogger<IStandControl> logger = Mock.Of<ILogger<IStandControl>>();
            this.standControl = new StandControl(realmControl);
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
