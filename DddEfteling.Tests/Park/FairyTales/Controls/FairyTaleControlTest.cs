using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Tests.Park.FairyTales.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Controls
{
    public class FairyTaleControlTest
    {
        IFairyTaleControl fairyTaleControl;

        public FairyTaleControlTest()
        {
            IRealmControl realmControl = new RealmControl();
            ILogger<FairyTaleControl> logger = Mock.Of<ILogger<FairyTaleControl>>();
            IMediator mediator = Mock.Of<IMediator>();
            this.fairyTaleControl = new FairyTaleControl(realmControl, logger, mediator);

        }

        [Fact]
        public void FindFairyTaleByName_FindSneeuwwitje_ExpectFairyTale()
        {
           
            FairyTale tale = fairyTaleControl.FindFairyTaleByName("Sneeuwwitje");
            Assert.NotNull(tale);
            Assert.Equal("Sneeuwwitje", tale.Name);
        }

        [Fact]
        public void All_GetAllFairyTales_ExpectAllFairytales()
        {
            List<FairyTale> fairyTales = fairyTaleControl.All();

            Assert.NotEmpty(fairyTales);
            Assert.Single(fairyTales.Where(fairytale => fairytale.Name.Equals("Sneeuwwitje")));
        }

        [Fact]
        public void Random_GetRandomTale_ExpectRandomTale()
        {
            FairyTale tale = fairyTaleControl.GetRandom();
            Assert.NotNull(tale);
            Assert.Contains(tale, fairyTaleControl.All());
        }
    }
}
