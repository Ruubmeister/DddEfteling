using DddEfteling.Common.Controls;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Rides.Entities;
using DddEfteling.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DddEfteling.Park.Visitors.Entities
{
    public class Visitor
    {
        private readonly Random random;
        private readonly VisitorSettings visitorSettings;
        private readonly VisitorLocationSelector locationSelector;

        [JsonIgnore]
        public Dictionary<DateTime, ILocation> VisitedLocations { get; } = new Dictionary<DateTime, ILocation>();

        public Visitor() {
            this.Guid = Guid.NewGuid();
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

        public ILocation GetLastLocation()
        {
            if (this.VisitedLocations.Count < 1)
            {
                return null;
            }

            return VisitedLocations[VisitedLocations.Keys.Max()];
        }

        public void AddVisitedLocation(ILocation location)
        {
            if (!VisitedLocations.ContainsValue(location)) 
            {

                if(VisitedLocations.Count >= 10)
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
        public ILocation TargetLocation { get; set; }

        public void WatchFairyTale(FairyTale tale)
        {
            int visitingSeconds = random.Next(visitorSettings.FairyTaleMinVisitingSeconds, visitorSettings.FairyTaleMaxVisitingSeconds);
            this.locationSelector.ReduceAndBalance(LocationType.FAIRYTALE);
            this.AddVisitedLocation(tale);
            this.TargetLocation = null;
            tale.AddVisitor(this, DateTime.Now.AddSeconds(visitingSeconds));
        }

        public void StepInRide(Ride ride)
        {
            ride.AddVisitorToLine(this);
            this.locationSelector.ReduceAndBalance(LocationType.RIDE);
            this.AddVisitedLocation(ride);
            this.TargetLocation = null;
        }
        
        public void WalkToDestination(double step)
        {
                this.CurrentLocation = CoordinateExtensions.GetStepCoordinates(CurrentLocation, TargetLocation.Coordinates, step);

        }
    }
}
