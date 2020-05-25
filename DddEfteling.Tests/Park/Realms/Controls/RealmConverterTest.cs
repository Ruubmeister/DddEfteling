using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Realms.Controls
{
    public class RealmConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectRealms()
        {
            string json = "[{\"name\": \"realm 1\"},{\"name\": \"realm 2\"}]";
            List<Realm> realms = JsonConvert.DeserializeObject<List<Realm>>(json);
            Assert.Equal(2, realms.Count);
            Assert.Equal("realm 1", realms.First().Name);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectRealms()
        {
            string json = "[{\"nam\": \"realm 1\"},{\"name\": \"realm 2\"}]";
            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<Realm>>(json));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            RealmConverter realmConverter = new RealmConverter();
            Assert.False(realmConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForRealm_expectTrue()
        {
            RealmConverter realmConverter = new RealmConverter();
            Realm realm = new Realm("realm");
            Assert.True(realmConverter.CanConvert(realm.GetType()));
        }

    }
}
