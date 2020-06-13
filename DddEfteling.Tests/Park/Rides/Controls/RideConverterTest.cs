using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Rides.Controls
{
    public class RideConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectRides()
        {
            string json = "[{\"status\": \"Closed\",\"name\": \"Carnaval Festival\",\"minimumAge\": \"0\"," +
                "\"minimumLength\": \"0\",\"duration\": {\"minutes\": 8,\"seconds\": 0},\"maxPersons\": 200," +
                "\"realm\": \"Reizenrijk\",\"coordinates\":{\"lat\":53.44,\"long\":5.443}}," +
                "{\"status\": \"Closed\",\"name\": \"Python\",\"minimumAge\": \"0\"," +
                "\"minimumLength\": \"0\",\"duration\": {\"minutes\": 8,\"seconds\": 0},\"maxPersons\": 200," +
                "\"realm\": \"Reizenrijk\",\"coordinates\":{\"lat\":53.44,\"long\":5.443}}]";

            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Reizenrijk");
            mock.Setup(r => r.FindRealmByName("Reizenrijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter(mock.Object));
            List<Ride> rides = JsonConvert.DeserializeObject<List<Ride>>(json, settings);
            Assert.Equal(2, rides.Count);
            Assert.Equal("Carnaval Festival", rides.First().Name);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectRealms()
        {
            string json = "[{\"status\": \"Closed\",\"nam\": \"Carnaval Festival\",\"minimumAge\": \"0\"," +
                "\"minimumLength\": \"0\",\"duration\": {\"minutes\": 8,\"seconds\": 0},\"maxPersons\": 200," +
                "\"realm\": \"Reizenrijk\",\"coordinates\":{\"lat\":53.44,\"long\":5.443}}]";
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Reizenrijk");
            mock.Setup(r => r.FindRealmByName("Reizenrijk")).Returns(realm);

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new RideConverter(mock.Object));

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<Ride>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            var mock = new Mock<IRealmControl>();
            RideConverter rideConverter = new RideConverter(mock.Object);
            Assert.False(rideConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForRealm_expectTrue()
        {
            var mock = new Mock<IRealmControl>();
            Realm realm = new Realm("Test realm");
            Coordinates coordinates = new Coordinates(1.22D, 45.44D);
            RideConverter rideConverter = new RideConverter(mock.Object);
            Ride ride = new Ride(RideStatus.Open, realm, coordinates, "Rider", 8, 1.33, TimeSpan.FromSeconds(31), 22);
            Assert.True(rideConverter.CanConvert(ride.GetType()));
        }
    }
}
