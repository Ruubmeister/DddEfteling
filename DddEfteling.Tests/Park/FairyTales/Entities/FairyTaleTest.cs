using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using System;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Entities
{
    public class FairyTaleTest
    {
        [Fact]
        public void Construct_CreateFairyTale_ExpectFairyTale()
        {
            Realm realm = new Realm("Test");
            FairyTale tale = new FairyTale("Sneeuwwitje", realm, new Coordinate(1.88, 2.77));

            Assert.Equal(realm, tale.Realm);
            Assert.Equal( "Sneeuwwitje", tale.Name);
            Assert.Equal(1.88, tale.Coordinates.Latitude);
            Assert.Equal(2.77, tale.Coordinates.Longitude);
            Assert.Equal(LocationType.FAIRYTALE, tale.LocationType);
            Assert.Empty(tale.VisitorWithTimeDone);
        }

        [Fact]
        public void VisitorWithTimeDone_TwoVisitors_ExpectBothFinallyDone()
        {
            Visitor visitor1 = new Visitor();
            Visitor visitor2 = new Visitor();

            FairyTale tale = new FairyTale();

            tale.AddVisitor(visitor1.Guid, DateTime.Now);
            tale.AddVisitor(visitor2.Guid, DateTime.Now.AddSeconds(1));

            Assert.Single(tale.GetVisitorsDone());
            System.Threading.Tasks.Task.Delay(1000).Wait();
            Assert.Single(tale.GetVisitorsDone());

        }
    }
}
