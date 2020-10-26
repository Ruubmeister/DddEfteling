using DddEfteling.Stands.Entities;
using Geolocation;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.Tests.Park.Stands.Entities
{
    public class StandTest
    {
        [Fact]
        public void Construct_CreateStandWithoutProducts_ExpectStand()
        {
            Coordinate coordinates = new Coordinate(1.2, 2.2);
            Stand stand = new Stand("Stand 1", coordinates, new List<Product>());

            Assert.Equal("Stand 1", stand.Name);
            Assert.Equal(coordinates, stand.Coordinates);
            Assert.Empty(stand.Meals);
            Assert.Empty(stand.Drinks);
        }

        [Fact]
        public void Construct_CreateStandWithProducts_ExpectStand()
        {
            Coordinate coordinates = new Coordinate(1.2, 2.2);

            List<Product> products = new List<Product>();
            products.Add(new Product("meal 1", 1.1F, ProductType.Meal));
            products.Add(new Product("meal 2", 1.2F, ProductType.Meal));
            products.Add(new Product("drink 1", 1.3F, ProductType.Drink));
            products.Add(new Product("drink 2", 1.4F, ProductType.Drink));
            products.Add(new Product("drink 3", 1.5F, ProductType.Drink));

            Stand stand = new Stand("Stand 1", coordinates, products);

            Assert.Equal(2, stand.Meals.Count);
            Assert.Equal(3, stand.Drinks.Count);

            Assert.Equal(1.1F, stand.Meals.First(meal => meal.Name.Equals("meal 1")).Price);
            Assert.Equal(1.2F, stand.Meals.First(meal => meal.Name.Equals("meal 2")).Price);
            Assert.Equal(1.5F, stand.Drinks.First(meal => meal.Name.Equals("drink 3")).Price);
        }
    }
}
