using DddEfteling.Shared.Boundaries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Stands.Entities
{
    public class Dinner
    {
        public Dinner() {
            this.Meals = new HashSet<Product>();
            this.Drinks = new HashSet<Product>();
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
            this.Meals = meals;
            this.Drinks = drinks;
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
            this.Meals = new HashSet<Product>(meals);
            this.Drinks = new HashSet<Product>(drinks);
        }

        public bool IsValid()
        {
            if(this.Meals.Count < 1 && this.Drinks.Count < 1)
            {
                return false;
            }

            return true;
        }

        public HashSet<Product> Meals { get; }

        public HashSet<Product> Drinks { get; }

        public DinnerDto ToDto()
        {
            return new DinnerDto(this.Meals.ToList().ConvertAll(m => m.Name), this.Drinks.ToList().ConvertAll(m => m.Name));
        }
    }
}
