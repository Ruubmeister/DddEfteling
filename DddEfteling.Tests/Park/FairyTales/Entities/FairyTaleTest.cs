using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Entities;
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
            FairyTale tale = new FairyTale("Sneeuwwitje", realm, new Coordinates(1.88, 2.77));

            Assert.Equal(realm, tale.Realm);
            Assert.Equal( "Sneeuwwitje", tale.Name);
            Assert.Equal(1.88, tale.Coordinates.Latitude);
            Assert.Equal(2.77, tale.Coordinates.Longitude);
        }
    }
}
