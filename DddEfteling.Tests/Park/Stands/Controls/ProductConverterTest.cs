using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Controls;
using DddEfteling.Park.Realms.Entities;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Stands.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Controls
{
    public class ProductConverterTest
    {
        [Fact]
        public void ReadJson_getCorrectJson_expectStands()
        {
            string json = "[{\"name\": \"kroket\",\"price\": 1.22, \"type\": \"meal\"}," +
                "{\"name\": \"7 up\",\"price\": 1.54, \"type\": \"drink\"}," +
                "{\"name\": \"Frietje met\",\"price\": 2.50, \"type\": \"meal\"}]";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ProductConverter());
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json, settings);
            Assert.Equal(3, products.Count);
            Assert.NotNull(products.FirstOrDefault(product => product.Name == "kroket"));
            Assert.NotNull(products.FirstOrDefault(product => product.Name == "7 up"));
            Assert.NotNull(products.FirstOrDefault(product => product.Name == "Frietje met"));
            Assert.Equal(1.54F, products.First(product => product.Name == "7 up").Price);
            Assert.Equal(ProductType.Meal, products.First(product => product.Name == "Frietje met").Type);
            Assert.Equal(ProductType.Drink, products.First(product => product.Name == "7 up").Type);
        }

        [Fact]
        public void ReadJson_getIncorrectJson_expectRealms()
        {
            string json = "[{\"nam\": \"kroket\",\"price\": 1.22, \"type\": \"meal\"}," +
                "{\"name\": \"7 up\",\"price\": 1.54, \"type\": \"drink\"}," +
                "{\"name\": \"Frietje met\",\"price\": 2.50, \"type\": \"meal\"}]";

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new ProductConverter());

            Assert.Throws<NullReferenceException>(() => JsonConvert.DeserializeObject<List<Product>>(json, settings));
        }

        [Fact]
        public void CanWrite_callFunction_expectFalse()
        {
            ProductConverter productConverter = new ProductConverter();
            Assert.False(productConverter.CanWrite);
        }

        [Fact]
        public void CanConvert_checkForRealm_expectTrue()
        {
            ProductConverter productConverter = new ProductConverter();
            Product product= new Product("Name", 1.24F, ProductType.Drink);
            Assert.True(productConverter.CanConvert(product.GetType()));
        }
    }
}
