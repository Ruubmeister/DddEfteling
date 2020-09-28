using DddEfteling.Shared.Boundary;
using Geolocation;
using System.Collections.Generic;

namespace DddEfteling.Stands.Entities
{
    public class Stand
    {

        public Stand(string name, Coordinate coordinates, List<Product> products)
        {
            Name = name;
            Coordinates = coordinates;
            Meals = products.FindAll(product => product.Type.Equals(ProductType.Meal));
            Drinks = products.FindAll(product => product.Type.Equals(ProductType.Drink));
        }

        public string Name { get; }

        public List<Product> Meals { get; }

        public List<Product> Drinks { get; }

        public Coordinate Coordinates { get; }

        public StandDto ToDto()
        {
            return new StandDto(this.Name, this.Meals.ConvertAll(meal => meal.Name), this.Drinks.ConvertAll(drink => drink.Name));
        }
    }
}
