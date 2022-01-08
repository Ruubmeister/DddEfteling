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

        public List<string> Meals { get; set; } = new ();
        public List<string> Drinks { get; set; } = new ();

        public StandDto()
        {
            LocationType = LocationType.STAND;
        }

        public StandDto(Guid guid, string name, Coordinate coodinates, List<string> meals, List<string> drinks)
        {
            LocationType = LocationType.STAND;
            
            Guid = guid;
            Name = name;
            Meals = meals;
            Drinks = drinks;
            Coordinates = coodinates;
        }
    }
}
