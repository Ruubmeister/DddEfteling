using DddEfteling.Shared.Entities;
using DddEfteling.Stands.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Stands.Controls
{
    public class DinnerBuilder
    {
        public Stand Stand { get; set; }

        public int TotalMeals { get; set; }

        private HashSet<Product> meals = new HashSet<Product>();

        public int TotalDrinks { get; set; }

        private HashSet<Product> drinks = new HashSet<Product>();

        private Random Random { get; } = new Random();

        public void BuildMeals()
        {
            meals = new HashSet<Product>();
            int maxStandMeals = Stand.Meals.Count();

            for (int i = 0; i < TotalMeals; i++)
            {
                meals.Add(Stand.Meals.ElementAt(Random.Next(maxStandMeals - 1)));
            }
        }

        public void BuildDrinks()
        {
            drinks = new HashSet<Product>();
            int maxStandDrinks = Stand.Drinks.Count();

            for (int i = 0; i < TotalDrinks; i++)
            {
                drinks.Add(Stand.Drinks.ElementAt(Random.Next(maxStandDrinks - 1)));
            }
        }

        public Dinner GetDinner()
        { 
            return new Dinner(meals, drinks);
        }
    }
}
