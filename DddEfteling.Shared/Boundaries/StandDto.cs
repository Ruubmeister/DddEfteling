using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;

namespace DddEfteling.Shared.Boundaries
{
    public class StandDto : ILocationDto
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public Coordinate Coordinates { get; set; }
        public LocationType LocationType { get; set; }

        public List<string> Meals { get; set; }
        public List<string> Drinks { get; set; }

        public StandDto()
        {
            LocationType = LocationType.STAND;
        }

        public StandDto(Guid guid, string name, Coordinate coodinates, List<string> meals, List<string> drinks)
        {
            this.Guid = guid;
            this.Name = name;
            this.Meals = meals;
            this.Drinks = drinks;
            this.Coordinates = coodinates;
        }
    }
}
