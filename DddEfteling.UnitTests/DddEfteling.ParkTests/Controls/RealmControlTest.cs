using DddEfteling.Park.Controls;
using DddEfteling.Park.Entities;
using Xunit;

namespace DddEfteling.ParkTests.Controls
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
