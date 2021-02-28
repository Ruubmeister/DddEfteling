using System;
using System.Collections.Generic;
using DddEfteling.Shared.Boundaries;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class DinnerDtoTest
    {
        [Fact]
        public void Construct_GivenEmptyDto_ExpectEmptyDto()
        {
            DinnerDto dinnerDto = new DinnerDto();
            
            Assert.NotNull(dinnerDto);
            Assert.Empty(dinnerDto.Drinks);
            Assert.Empty(dinnerDto.Meals);
        }
        
        [Fact]
        public void Construct_GivenDtoWithProducts_ExpectDto()
        {
            var meals = new List<String>()
            {
                "Meal 1",
                "Meal 2"
            };
            var drinks = new List<String>()
            {
                "Drink 1",
                "Drink 2",
                "Drink 3"
            };
            
            DinnerDto dinnerDto = new DinnerDto(meals, drinks);
            
            Assert.NotNull(dinnerDto);
            Assert.Equal(2, dinnerDto.Meals.Count);
            Assert.Equal(3, dinnerDto.Drinks.Count);
        }
    }
}