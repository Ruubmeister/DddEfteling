using DddEfteling.Park.Entities;
using Xunit;

namespace DddEfteling.ParkTests.Entities
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
