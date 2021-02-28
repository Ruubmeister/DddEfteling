using System;
using System.Collections.Generic;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Stands.Entities;
using Xunit;

namespace DddEfteling.StandTests.Entities
{
    public class DinnerTest
    {
        [Fact]
        public void Construct_CreateDinner_ExpectEmptyDinner()
        {
            Dinner dinner = new Dinner();
            Assert.Empty(dinner.Drinks);
            Assert.Empty(dinner.Meals);
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsSet_ExpectDinnerWithMealsAndDrinks()
        {
            var meals = new HashSet<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new HashSet<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Dinner dinner = new Dinner(meals, drinks);
            Assert.Equal(drinks, dinner.Drinks);
            Assert.Equal(meals, dinner.Meals);
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsList_ExpectDinnerWithMealsAndDrinks()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Dinner dinner = new Dinner(meals, drinks);
            Assert.Equal(drinks, dinner.Drinks);
            Assert.Equal(meals, dinner.Meals);
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsSetWithIncorrectMealType_ExpectException()
        {
            var meals = new HashSet<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Drink)
            };
            var drinks = new HashSet<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Assert.Throws<ArgumentException>( () => new Dinner(meals, drinks));
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsListWithIncorrectMealType_ExpectException()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Drink)
            };
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Assert.Throws<ArgumentException>( () => new Dinner(meals, drinks));
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsSetWithIncorrectDrinkType_ExpectException()
        {
            var meals = new HashSet<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new HashSet<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Meal),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Assert.Throws<ArgumentException>( () => new Dinner(meals, drinks));
        }
        
        [Fact]
        public void Construct_CreatedWithMealsAndDrinksAsListWithIncorrectDrinkType_ExpectException()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Meal),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Assert.Throws<ArgumentException>( () => new Dinner(meals, drinks));
        }
        
        [Fact]
        public void ToDto_GivenDinner_ExpectCorrectDto()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            var dinner = new Dinner(meals, drinks);

            var dto = dinner.ToDto();
            Assert.NotNull(dinner);
            Assert.Equal(2, dto.Meals.Count);
            Assert.Equal(3, dto.Drinks.Count);
            Assert.Contains("meal 2", dto.Meals);
            Assert.Contains("drink 3", dto.Drinks);
        }

        [Fact]
        public void IsValid_GivenProductWithMealsAndDrinks_ExpectTrue()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Dinner dinner = new Dinner(meals, drinks);
            Assert.True(dinner.IsValid());
        }
        
        [Fact]
        public void IsValid_GivenProductWithMeals_ExpectTrue()
        {
            var meals = new List<Product>()
            {
                new Product("meal 1", 0.00F, ProductType.Meal),
                new Product("meal 2", 0.00F, ProductType.Meal)
            };
            var drinks = new List<Product>();
            Dinner dinner = new Dinner(meals, drinks);
            Assert.True(dinner.IsValid());
        }
        
        [Fact]
        public void IsValid_GivenProductWithDrinks_ExpectTrue()
        {
            var meals = new List<Product>();
            var drinks = new List<Product>()
            {
                new Product("drink 1", 0.00F, ProductType.Drink),
                new Product("drink 2", 0.00F, ProductType.Drink),
                new Product("drink 3", 0.00F, ProductType.Drink)
            };
            Dinner dinner = new Dinner(meals, drinks);
            Assert.True(dinner.IsValid());
        }
        
        [Fact]
        public void IsValid_GivenNoProducts_ExpectFalse()
        {
            Dinner dinner = new Dinner();
            Assert.False(dinner.IsValid());
        }
    }
}