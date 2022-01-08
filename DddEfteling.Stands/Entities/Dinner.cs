using DddEfteling.Shared.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Stands.Entities
{
    public class Dinner
    {
        public Dinner() {
            Meals = new HashSet<Product>();
            Drinks = new HashSet<Product>();
        }

        public Dinner(HashSet<Product> meals, HashSet<Product> drinks) { 
            if(meals.Any(item => !item.Type.Equals(ProductType.Meal)))
            {
                throw new ArgumentException("Given meals contain other types");
            }
            if (drinks.Any(item => !item.Type.Equals(ProductType.Drink)))
            {
                throw new ArgumentException("Given meals contain other types");
            }
            Meals = meals;
            Drinks = drinks;
        }

        public Dinner(List<Product> meals, List<Product> drinks) {
            if(meals.Any(item => !item.Type.Equals(ProductType.Meal)))
            {
                throw new ArgumentException("Given meals contain other types");
            }
            if (drinks.Any(item => !item.Type.Equals(ProductType.Drink)))
            {
                throw new ArgumentException("Given meals contain other types");
            }
            Meals = new HashSet<Product>(meals);
            Drinks = new HashSet<Product>(drinks);
        }

        public bool IsValid()
        {
            return Meals.Count >= 1 || Drinks.Count >= 1;
        }

        public HashSet<Product> Meals { get; }

        public HashSet<Product> Drinks { get; }

        public DinnerDto ToDto()
        {
            return new DinnerDto(Meals.ToList().ConvertAll(m => m.Name), Drinks.ToList().ConvertAll(m => m.Name));
        }
    }
}
