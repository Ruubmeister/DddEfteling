using DddEfteling.Common.Controls;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.FairyTales.Controls;
using DddEfteling.Park.FairyTales.Entities;
using DddEfteling.Park.Rides.Controls;
using DddEfteling.Park.Rides.Entities;
using DddEfteling.Park.Stands.Controls;
using DddEfteling.Park.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.Visitors.Controls
{
    public class VisitorControl : IVisitorControl
    {
        private readonly Random random = new Random();
        private readonly IMediator mediator;
        private readonly IOptions<VisitorSettings> visitorSettings;
        private readonly Coordinate startCoordinate = new Coordinate(51.649175, 5.045545);
        private readonly ILogger<VisitorControl> logger;
        private readonly IFairyTaleControl fairyTaleControl;
        private readonly IRideControl rideControl;
        private readonly IStandControl standControl;

        private Dictionary<Guid, DateTime> IdleVisitors = new Dictionary<Guid, DateTime>();

        private List<Visitor> Visitors { get; } = new List<Visitor>();

        public VisitorControl(IMediator mediator, IOptions<VisitorSettings> settings, ILogger<VisitorControl> logger,
            IFairyTaleControl fairyTaleControl, IRideControl rideControl, IStandControl standControl)
        {
            this.mediator = mediator;
            this.visitorSettings = settings;
            this.logger = logger;
            this.fairyTaleControl = fairyTaleControl;
            this.rideControl = rideControl;
            this.standControl = standControl;
        }

        public void HandleIdleVisitors()
        {
            List<KeyValuePair<Guid, DateTime>> currentList = IdleVisitors.ToList();
            IdleVisitors.Clear();

            foreach (KeyValuePair<Guid, DateTime> visitorAtTime in currentList.AsEnumerable())
            {
                Visitor visitor = Visitors.First(visitor => visitor.Guid.Equals(visitorAtTime.Key));

                double step = (double)random.Next(83, 166) / 100;
                TimeSpan timeIdle = DateTime.Now - visitorAtTime.Value;

                double correctedStep = timeIdle.TotalSeconds * step;

                if (CoordinateExtensions.IsInRange(visitor.CurrentLocation, visitor.TargetLocation.Coordinates, correctedStep))
                {
                    if (visitor.TargetLocation.LocationType.Equals(LocationType.RIDE))
                    {
                        visitor.StepInRide((Ride)visitor.TargetLocation);
                    }
                    else if (visitor.TargetLocation.LocationType.Equals(LocationType.FAIRYTALE))
                    {
                        visitor.WatchFairyTale((FairyTale)visitor.TargetLocation);
                    }
                }
                else
                {
                    visitor.WalkToDestination(correctedStep);

                    Dictionary<string, object> payload = new Dictionary<string, object>
                    {
                        { "DateTime", DateTime.Now }
                    };

                    this.mediator.Publish(new VisitorEvent(EventType.Idle, visitor.Guid, payload));
                }
            }
        }

        public List<Visitor> All()
        {
            return Visitors;
        }

        public void AddVisitors(int number)
        {
            for (int i = 1; i <= number; i++)
            {
                logger.LogDebug($"Adding {i} visitors");
                // Todo: Fix hardcoded below
                Visitor visitor = new Visitor(DateTime.Now, 1.55, startCoordinate, random, visitorSettings);
                Visitors.Add(visitor);
                KickOffVisitor(visitor);
            }
        }

        private void SetNewLocation(Visitor visitor)
        {

        }

        private void KickOffVisitor(Visitor visitor)
        {
            logger.LogInformation($"Kicking off visitor {visitor.Guid}");
            LocationType type = visitor.GetLocationType(null);

            switch (type)
            {
                case LocationType.FAIRYTALE:
                    FairyTale tale = fairyTaleControl.GetRandom();
                    visitor.TargetLocation = tale;
                    break;
                case LocationType.RIDE:
                    Ride ride = rideControl.GetRandom();
                    visitor.TargetLocation = ride;
                    break;
            }

            Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "DateTime", DateTime.Now }
                };

            this.mediator.Publish(new VisitorEvent(EventType.Idle, visitor.Guid, payload));

        }

        private string GetNewClosestToLocation(Visitor visitor, ILocation location)
        {
            return FilterNotVisited(visitor, location.DistanceToOthers.Keys.ToList()).First();
        }

        private List<string> FilterNotVisited(Visitor visitor, List<string> locationNames)
        {
            List<string> visitedLocations = visitor.VisitedLocations.Values.Select(visited => visited.Name).ToList();
            return locationNames.Where(loc => !visitedLocations.Contains(loc)).ToList();
        }
        public void AddIdleVisitor(Guid visitorGuid, DateTime dateTime)
        {
            this.IdleVisitors.Add(visitorGuid, dateTime);
        }
    }

    public interface IVisitorControl
    {
        public void AddVisitors(int number);
        public List<Visitor> All();

        public void AddIdleVisitor(Guid visitorGuid, DateTime dateTime);

        public void HandleIdleVisitors();
    }
}
