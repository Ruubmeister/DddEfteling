using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Controls
{
    public class StandControlTest
    {
        [Fact]
        public void FindRideByName_FindPollesPannenkoeken_ExpectStand()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<IStandControl> logger = Mock.Of<ILogger<IStandControl>>();
            StandControl standControl = new StandControl(realmControl);

            Stand stand = standControl.FindStandByName("Polles pannenkoeken");
            Assert.NotNull(stand);
            Assert.Equal("Polles pannenkoeken", stand.Name);
        }
    }
}
