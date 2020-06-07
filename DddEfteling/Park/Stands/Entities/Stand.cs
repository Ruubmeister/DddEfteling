using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Realms.Entities;
using System.Collections.Generic;

namespace DddEfteling.Park.Stands.Entities
{
    public class Stand
    {

        public Stand(string name, Realm realm, Coordinates coordinates, List<Product> products)
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

        public Coordinates Coordinates { get; }
    }
}
