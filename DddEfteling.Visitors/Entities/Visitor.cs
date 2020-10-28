using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Geolocation;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace DddEfteling.Visitors.Entities
{
    public class Visitor
    {
        private readonly Random random;
        private readonly VisitorSettings visitorSettings;
        private readonly VisitorLocationSelector locationSelector;

        [JsonIgnore]
        public Dictionary<DateTime, ILocationDto> VisitedLocations { get; } = new Dictionary<DateTime, ILocationDto>();

        public Visitor()
        {
            this.Guid = Guid.NewGuid();
            this.random = new Random();
            this.locationSelector = new VisitorLocationSelector(random);
        }

        public Visitor(DateTime dateOfBirth, double length, Coordinate startLocation, Random random,
            IOptions<VisitorSettings> visitorSettings)
        {
            Guid = Guid.NewGuid();
            DateOfBirth = dateOfBirth;
            Length = length;
            this.CurrentLocation = startLocation;
            this.random = random;
            this.visitorSettings = visitorSettings.Value;
            this.locationSelector = new VisitorLocationSelector(random);
        }

        public ILocationDto GetLastLocation()
        {
            if (this.VisitedLocations.Count < 1)
            {
                return null;
            }

            return VisitedLocations[VisitedLocations.Keys.Max()];
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
            return this.locationSelector.GetLocation(previousLocationType);
        }

        public Guid Guid { get; }

        public DateTime DateOfBirth { get; }

        public double Length { get; }

        public Coordinate CurrentLocation { get; set; }

        [JsonIgnore]
        public ILocationDto TargetLocation { get; set; }

        public void WatchFairyTale(FairyTaleDto tale)
        {
            int visitingSeconds = random.Next(visitorSettings.FairyTaleMinVisitingSeconds, visitorSettings.FairyTaleMaxVisitingSeconds);
            this.locationSelector.ReduceAndBalance(LocationType.FAIRYTALE);
            this.AddVisitedLocation(tale);
            this.TargetLocation = null;
        }

        public void StepInRide(RideDto ride)
        {
            this.locationSelector.ReduceAndBalance(LocationType.RIDE);
            this.AddVisitedLocation(ride);
            this.TargetLocation = null;
        }

        public void WalkToDestination(double step)
        {
            this.CurrentLocation = CoordinateExtensions.GetStepCoordinates(CurrentLocation, TargetLocation.Coordinates, step);
        }

        public VisitorDto ToDto()
        {
            return new VisitorDto(this.Guid, this.DateOfBirth, this.Length, this.CurrentLocation);
        }
    }
}
