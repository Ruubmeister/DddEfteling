using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public class StandDto : ILocationDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public LocationType LocationType { get; set; }

        public List<string> Meals { get; set; }
        public List<string> Drinks { get; set; }

        public StandDto() { }
        public StandDto(string name, Coordinate coodinates, List<string> meals, List<string> drinks)
        {
            this.Name = name;
            this.Meals = meals;
            this.Drinks = drinks;
            this.Coordinates = coodinates;
        }
    }
}
