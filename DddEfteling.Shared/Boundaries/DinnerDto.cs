using System.Collections.Generic;

namespace DddEfteling.Shared.Boundaries
{
    public class DinnerDto
    {
        public List<string> Meals { get; } = new ();

        public List<string> Drinks { get; } = new ();

        public DinnerDto() { }

        public DinnerDto(List<string> meals, List<string> drinks)
        {
            Meals = meals;
            Drinks = drinks;
        }
    }
}
