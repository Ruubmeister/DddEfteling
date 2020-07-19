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

        public Visitor() { }

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

        public ILocation TargetLocation { get; set; }

        public void WatchFairyTale(FairyTale tale)
        {
            int visitingSeconds = random.Next(visitorSettings.FairyTaleMinVisitingSeconds, visitorSettings.FairyTaleMaxVisitingSeconds);
            this.locationSelector.ReduceAndBalance(LocationType.FAIRYTALE);
            tale.AddVisitor(this.Guid, DateTime.Now.AddSeconds(visitingSeconds));
        }

        public void StepInRide(Ride ride)
        {
            ride.AddVisitorToLine(this);
            this.locationSelector.ReduceAndBalance(LocationType.RIDE);
            Task.Run(() => {
                    while (ride.HasVisitor(this))
                    {
                        Task.Delay(TimeSpan.FromSeconds(10)).Wait();
                    }
                }).Wait();
        }
        
        public void WalkToDestination(double step)
        {
                this.CurrentLocation = CoordinateExtensions.GetStepCoordinates(CurrentLocation, TargetLocation.Coordinates, step);

        }
    }
}
