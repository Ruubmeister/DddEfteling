using DddEfteling.Stands.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Shared.Entities
{
    public class Dinner
    {
        public Dinner() { }
        public Dinner(HashSet<Product> meals, HashSet<Product> drinks) { 
            if(meals.Where(item => !item.Type.Equals(ProductType.Meal)).Any())
            {
                throw new ArgumentException("Given meals contain other types");
            }
            if (drinks.Where(item => !item.Type.Equals(ProductType.Drink)).Any())
            {
                throw new ArgumentException("Given meals contain other types");
            }
            this.Meals = meals;
            this.Drinks = drinks;
        }

        public HashSet<Product> Meals { get; }

        public HashSet<Product> Drinks { get; }
    }
}
