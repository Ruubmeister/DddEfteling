using DddEfteling.FairyTales.Controls;
using DddEfteling.FairyTales.Entities;
using Geolocation;
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

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter());
            List<FairyTale> tales = JsonConvert.DeserializeObject<List<FairyTale>>(json, settings);
            Assert.Equal(2, tales.Count);
            Assert.Equal("Sneeuwwitje", tales.First().Name);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectException()
        {
            string json = "[{\"nam\": \"Sneeuwwitje\"}]";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new FairyTaleConverter());

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<FairyTale>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            FairyTaleConverter fairyTaleConverter = new FairyTaleConverter();
            Assert.False(fairyTaleConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForType_expectTrue()
        {
            FairyTaleConverter fairyTaleConverter = new FairyTaleConverter();
            FairyTale tale = new FairyTale("Sneeuwwitje", new Coordinate(1.2, 2.4));
            Assert.True(fairyTaleConverter.CanConvert(tale.GetType()));
        }
    }
}
