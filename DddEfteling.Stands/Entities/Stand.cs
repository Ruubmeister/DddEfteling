using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.Stands.Entities
{
    public class Stand : Location
    {

        public Stand(string name, Coordinate coordinates, List<Product> products) : base(Guid.NewGuid(), LocationType.FAIRYTALE)
        {
            Name = name;
            Coordinates = coordinates;
            Meals = products.FindAll(product => product.Type.Equals(ProductType.Meal));
            Drinks = products.FindAll(product => product.Type.Equals(ProductType.Drink));
        }

        public List<Product> Meals { get; }

        public List<Product> Drinks { get; }

        public StandDto ToDto()
        {
            return new StandDto(this.Guid, this.Name, this.Coordinates, this.Meals.ConvertAll(meal => meal.Name), this.Drinks.ConvertAll(drink => drink.Name));
        }
    }
}
