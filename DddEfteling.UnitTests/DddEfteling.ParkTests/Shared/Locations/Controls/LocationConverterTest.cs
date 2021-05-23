using System;
using System.Collections.Generic;
using System.Linq;
using DddEfteling.ParkTests.Entities;
using DddEfteling.Shared.Controls;
using Geolocation;
using Newtonsoft.Json;
using Xunit;

namespace DddEfteling.ParkTests.Controls
{
    public class LocationConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectlocation()
        {
            string json = "[{\"name\":\"Test\",\"coordinates\": {\"lat\": 1.4,\"long\": 1.54}}," +
                          "{\"name\":\"Test\",\"coordinates\": {\"lat\": 1.6,\"long\": 1.88}}]";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new LocationConverter<LocationImpl>((x) => new LocationImpl(x)));
            List<LocationImpl> locations = JsonConvert.DeserializeObject<List<LocationImpl>>(json, settings);
            Assert.Equal(2, locations.Count);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectException()
        {
            string json = "[{\"name\":\"Test\",\"coordinats\": {\"lat\": 1.4,\"long\": 1.54}}]";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new LocationConverter<LocationImpl>((x) => new LocationImpl(x)));

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<LocationImpl>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            LocationConverter<LocationImpl> locationConverter = new LocationConverter<LocationImpl>((x) => new LocationImpl(x));
            Assert.False(locationConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForType_expectTrue()
        {
            LocationConverter<LocationImpl> locationConverter = new LocationConverter<LocationImpl>((x) => new LocationImpl(x));
            LocationImpl location = new LocationImpl(new Coordinate(1.2, 2.4));
            Assert.True(locationConverter.CanConvert(location.GetType()));
        }
    }
}