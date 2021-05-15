using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DddEfteling.Stands.Entities
{
    public class Stand : ILocation
    {

        public Stand(string name, Coordinate coordinates, List<Product> products)
        {
            Name = name;
            Coordinates = coordinates;
            Meals = products.FindAll(product => product.Type.Equals(ProductType.Meal));
            Drinks = products.FindAll(product => product.Type.Equals(ProductType.Drink));
            LocationType = LocationType.STAND;
        }
        public LocationType LocationType { get; }

        public Guid Guid { get; } = Guid.NewGuid();

        public string Name { get; }

        public List<Product> Meals { get; }

        public List<Product> Drinks { get; }

        public Coordinate Coordinates { get; }

        [JsonIgnore]
        public SortedDictionary<double, Guid> DistanceToOthers { get; } = new SortedDictionary<double, Guid>();

        public double GetDistanceTo(Stand stand)
        {
            return GeoCalculator.GetDistance(this.Coordinates, stand.Coordinates, 4, DistanceUnit.Meters);
        }

        public void AddDistanceToOthers(double distance, Guid standGuid)
        {
            this.DistanceToOthers.Add(distance, standGuid);
        }

        public StandDto ToDto()
        {
            return new StandDto(this.Guid, this.Name, this.Coordinates, this.Meals.ConvertAll(meal => meal.Name), this.Drinks.ConvertAll(drink => drink.Name));
        }
    }
}
