using DddEfteling.Shared.Boundaries;
using Geolocation;
using System;
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

        public Guid Guid { get; } = Guid.NewGuid();

        public string Name { get; }

        public List<Product> Meals { get; }

        public List<Product> Drinks { get; }

        public Coordinate Coordinates { get; }

        public StandDto ToDto()
        {
            return new StandDto(this.Guid, this.Name, this.Coordinates, this.Meals.ConvertAll(meal => meal.Name), this.Drinks.ConvertAll(drink => drink.Name));
        }
    }
}
