using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Tests.Park.FairyTales.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.FairyTales.Controls
{
    public class FairyTaleConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectFairyTale()
        {
            string json = "[{\"name\": \"Sneeuwwitje\",\"realm\": \"Marerijk\",\"coordinates\": {\"lat\": 1.4,\"long\": 1.54}}," +
                "{\"name\": \"Doornroosje\",\"realm\": \"Marerijk\",\"coordinates\": {\"lat\": 1.6,\"long\": 1.88}}]";

            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Marerijk");
            mock.Setup(r => r.FindRealmByName("Marerijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter(mock.Object));
            List<FairyTale> tales = JsonConvert.DeserializeObject<List<FairyTale>>(json, settings);
            Assert.Equal(2, tales.Count);
            Assert.Equal("Sneeuwwitje", tales.First().Name);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectException()
        {
            string json = "[{\"nam\": \"Sneeuwwitje\"}]";
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Marerijk");
            mock.Setup(r => r.FindRealmByName("Marerijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter(mock.Object));

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<FairyTale>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            var mock = new Mock<IRealmControl>();
            FairyTaleConverter fairyTaleConverter = new FairyTaleConverter(mock.Object);
            Assert.False(fairyTaleConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForType_expectTrue()
        {
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Test realm");
            FairyTaleConverter fairyTaleConverter = new FairyTaleConverter(mock.Object);
            FairyTale tale = new FairyTale("Sneeuwwitje", realm, new Coordinates(1.2, 2.4));
            Assert.True(fairyTaleConverter.CanConvert(tale.GetType()));
        }
    }
}
