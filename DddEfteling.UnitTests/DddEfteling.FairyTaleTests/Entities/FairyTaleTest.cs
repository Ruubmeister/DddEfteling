
using DddEfteling.FairyTales.Entities;
using DddEfteling.Shared.Entities;
using Geolocation;
using Xunit;

namespace DddEfteling.FairyTaleTests.Entities
{
    public class FairyTaleTest
    {
        [Fact]
        public void Construct_CreateFairyTale_ExpectFairyTale()
        {
            FairyTale tale = new FairyTale("Sneeuwwitje", new Coordinate(1.88, 2.77));

            Assert.Equal("Sneeuwwitje", tale.Name);
            Assert.Equal(1.88, tale.Coordinates.Latitude);
            Assert.Equal(2.77, tale.Coordinates.Longitude);
            Assert.Equal(LocationType.FAIRYTALE, tale.LocationType);
            Assert.Empty(tale.VisitorWithTimeDone);
        }
    }
}
