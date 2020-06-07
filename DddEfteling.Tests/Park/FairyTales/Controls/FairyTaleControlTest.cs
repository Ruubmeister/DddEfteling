using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Tests.Park.FairyTales.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Controls
{
    public class FairyTaleControlTest
    {
        [Fact]
        public void FindFairyTaleByName_FindSneeuwwitje_ExpectFairyTale()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            FairyTaleControl fairyTaleControl = new FairyTaleControl(realmControl, logger);

            FairyTale tale = fairyTaleControl.FindFairyTaleByName("Sneeuwwitje");
            Assert.NotNull(tale);
            Assert.Equal("Sneeuwwitje", tale.Name);
        }
    }
}
