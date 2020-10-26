using DddEfteling.Park.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Realms.Entities
{
    public class RealmTest
    {
        [Fact]
        public void constructor_realmCreated_expectsRealm()
        {
            Realm realm = new Realm("Marerijk");
            Assert.Equal("Marerijk", realm.Name);
        }
    }
}
