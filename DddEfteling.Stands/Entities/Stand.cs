using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DddEfteling.Stands.Entities
{
    public class Stand : Location
    {
        
        public Stand(): base(Guid.NewGuid(), LocationType.STAND){}

        public Stand(string name, Coordinate coordinates, List<Product> products) : base(Guid.NewGuid(), LocationType.STAND)
        {
            Name = name;
            Coordinates = coordinates;
            Meals = products.FindAll(product => product.Type.Equals(ProductType.Meal));
            Drinks = products.FindAll(product => product.Type.Equals(ProductType.Drink));
        }

        public Stand(JObject obj): base(Guid.NewGuid(), LocationType.STAND)
        {
            
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(obj["products"].ToString());

            Name = obj["name"].ToString();
            Coordinates = new Coordinate(
                double.Parse(obj["coordinates"]["lat"].ToString()),
                double.Parse(obj["coordinates"]["long"].ToString()));
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
