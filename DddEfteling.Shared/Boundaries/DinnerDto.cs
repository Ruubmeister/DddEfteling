using System.Collections.Generic;

namespace DddEfteling.Shared.Boundaries
{
    public class DinnerDto
    {
        public List<string> Meals { get; } = new List<string>();

        public List<string> Drinks { get; } = new List<string>();

        public DinnerDto() { }

        public DinnerDto(List<string> meals, List<string> drinks)
        {
            this.Meals = meals;
            this.Drinks = drinks;
        }
    }
}
