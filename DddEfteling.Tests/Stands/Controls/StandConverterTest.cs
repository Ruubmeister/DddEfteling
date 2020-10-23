using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
using Geolocation;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Controls
{
    public class StandConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectStands()
        {
            string json = "[{\"name\": \"Friettent\",\"realm\": \"Reizenrijk\",\"coordinates\": {\"lat\": 1.2, \"long\":2.2}," +
                "\"products\":[{\"name\": \"kroket\",\"price\": 1.22, \"type\": \"meal\"}," +
                "{\"name\": \"7 up\",\"price\": 1.54, \"type\": \"drink\"},{\"name\": \"Frietje met\",\"price\": 2.50, \"type\": \"meal\"}]}]";

            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Reizenrijk");
            mock.Setup(r => r.FindRealmByName("Reizenrijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StandConverter(mock.Object));
            List<Stand> stands = JsonConvert.DeserializeObject<List<Stand>>(json, settings);
            Assert.Single(stands);
            Assert.Equal("Friettent", stands.First().Name);
            Assert.Equal(2, stands.First().Meals.Count);
            Assert.Single(stands.First().Drinks);
            Assert.Contains(stands.First().Meals, meal => meal.Name.Equals("Frietje met"));
            Assert.Contains(stands.First().Meals, meal => meal.Name.Equals("kroket"));
            Assert.Contains(stands.First().Drinks, drink => drink.Name.Equals("7 up"));
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectRealms()
        {
            string json = "[{\"nam\": \"Friettent\",\"realm\": \"Reizenrijk\",\"coordinates\": {\"lat\": 1.2, \"long\":2.2}," +
                "\"products\":[{\"name\": \"kroket\",\"price\": 1.22, \"type\": \"meal\"}," +
                "{\"name\": \"7 up\",\"price\": 1.54, \"type\": \"drink\"},{\"name\": \"Frietje met\",\"price\": 2.50, \"type\": \"meal\"}]}]";
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Reizenrijk");
            mock.Setup(r => r.FindRealmByName("Reizenrijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new StandConverter(mock.Object));

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<Stand>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            var mock = new Mock<IRealmControl>();
            StandConverter standConverter = new StandConverter(mock.Object);
            Assert.False(standConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForRealm_expectTrue()
        {
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Test realm");
            StandConverter standConverter = new StandConverter(mock.Object);
            Stand stand = new Stand("Name", realm, new Coordinate(), new List<Product>());
            Assert.True(standConverter.CanConvert(stand.GetType()));
        }
    }
}
