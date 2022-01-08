using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Geolocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DddEfteling.Visitors.Controls;

namespace DddEfteling.Visitors.Entities
{
    public class Visitor
    {
        private readonly Random random;
        private readonly VisitorLocationSelector locationSelector;

        [JsonIgnore]
        public Dictionary<DateTime, ILocationDto> VisitedLocations { get; } = new ();

        public Visitor()
        {
            Guid = Guid.NewGuid();
            random = new Random();
            locationSelector = new VisitorLocationSelector(random);
        }

        public Visitor(DateTime dateOfBirth, double length, Coordinate startLocation, Random random)
        {
            Guid = Guid.NewGuid();
            DateOfBirth = dateOfBirth;
            Length = length;
            CurrentLocation = startLocation;
            this.random = random;
            locationSelector = new VisitorLocationSelector(random);
        }

        public Guid Guid { get; }

        public DateTime DateOfBirth { get; }

        public double Length { get; }

        public Coordinate CurrentLocation { get; set; }

        [JsonIgnore]
        public ILocationDto TargetLocation { get; set; }
        
        public DateTime? AvailableAt { get; set; }

        public IVisitorLocationStrategy LocationStrategy { get; set; }
        
        public double NextStepDistance { get; set; }

        public void DoActivity( ILocationDto locationDto)
        {
            locationSelector.ReduceAndBalance(locationDto.LocationType);
            AddVisitedLocation(locationDto);
            TargetLocation = null;
        }

        public void WalkToDestination()
        {
            CurrentLocation = CoordinateExtensions.GetStepCoordinates(CurrentLocation, TargetLocation.Coordinates, NextStepDistance);
        }
        
        public ILocationDto GetLastLocation()
        {
            return (VisitedLocations.Count < 1) ? null : VisitedLocations[VisitedLocations.Keys.Max()];
        }

        public List<string> PickStandProducts(StandDto stand)
        {
            var products = new List<string>
            {
                stand.Meals.OrderBy(x => Guid.NewGuid()).FirstOrDefault(),
                stand.Drinks.OrderBy(x => Guid.NewGuid()).FirstOrDefault()
            };

            products.RemoveAll(product => product == null);

            return products;
        }

        public void PickUpOrder(IStandClient client, string order)
        {
            var dinner = client.GetOrder(order);

            if(dinner != null)
            {
                TargetLocation = null;
            }
        }

        public void AddVisitedLocation(ILocationDto location)
        {
            if (!VisitedLocations.ContainsValue(location))
            {

                if (VisitedLocations.Count >= 10)
                {
                    VisitedLocations.Remove(VisitedLocations.Keys.Min());
                }

                VisitedLocations.Add(DateTime.Now, location);
            }
        }

        public LocationType GetLocationType(LocationType? previousLocationType)
        {
            return locationSelector.GetLocation(previousLocationType);
        }

        public VisitorDto ToDto()
        {
            return new VisitorDto(Guid, DateOfBirth, Length, CurrentLocation);
        }
    }
}
