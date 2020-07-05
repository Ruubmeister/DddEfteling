using DddEfteling.Park.Realms.Entities;
using Geolocation;
using System.Collections.Generic;

namespace DddEfteling.Park.Stands.Entities
{
    public class Stand
    {

        public Stand(string name, Realm realm, Coordinate coordinates, List<Product> products)
        {
            Name = name;
            Realm = realm;
            Coordinates = coordinates;
            Meals = products.FindAll(product => product.Type.Equals(ProductType.Meal));
            Drinks = products.FindAll(product => product.Type.Equals(ProductType.Drink));
        }

        public string Name { get; }

        public List<Product> Meals { get; }

        public List<Product> Drinks { get; }

        public Realm Realm { get; }

        public Coordinate Coordinates { get; }
    }
}
