﻿using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using Xunit;

namespace DddEfteling.Tests.Park.Realms.Controls
{
    public class RealmControlTest
    {
        [Fact]
        public void FindRealmByName_GetMarerijk_ExpectRealm()
        {
            RealmControl realmControl = new RealmControl();
            Realm realm = realmControl.FindRealmByName("Marerijk");
            Assert.NotNull(realm);
            Assert.Equal("Marerijk", realm.Name);
        }
    }
}