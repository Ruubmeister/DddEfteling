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
using System.Collections.Concurrent;
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

        private ConcurrentBag<Visitor> Visitors { get; } = new ConcurrentBag<Visitor>();

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
            if (IdleVisitors.Count < 1)
            {
                this.logger.LogInformation("No idle visitors found");
                return;
            }

            this.logger.LogDebug("Parsing idle visitors");

            Dictionary<Guid, DateTime> copiedList = new Dictionary<Guid, DateTime>(IdleVisitors);
            IdleVisitors.Clear();

            foreach (KeyValuePair<Guid, DateTime> visitorAtTime in copiedList.AsEnumerable())
            {
                Visitor visitor = Visitors.ToList().First(visitor => visitor.Guid.Equals(visitorAtTime.Key));
                if(visitor.TargetLocation == null)
                {
                    this.SetNewLocation(visitor);
                }

                double step = (double)random.Next(50, 150) / 100;
                TimeSpan timeIdle = DateTime.Now - visitorAtTime.Value;

                double correctedStep = timeIdle.TotalSeconds * step;

                if (CoordinateExtensions.IsInRange(visitor.CurrentLocation, visitor.TargetLocation.Coordinates, correctedStep))
                {
                    if (visitor.TargetLocation.LocationType.Equals(LocationType.RIDE))
                    {
                        this.logger.LogInformation($"Visitor {visitor.Guid} stepping into ride {visitor.TargetLocation.Name}");
                        visitor.StepInRide((Ride)visitor.TargetLocation);
                    }
                    else if (visitor.TargetLocation.LocationType.Equals(LocationType.FAIRYTALE))
                    {
                        this.logger.LogInformation($"Visitor {visitor.Guid} watching fairytale {visitor.TargetLocation.Name}");
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
            return Visitors.ToList();
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

        public void SetNewLocation(Visitor visitor)
        {
            ILocation previousLocation = visitor.GetLastLocation();

            LocationType type = visitor.GetLocationType(previousLocation?.LocationType);
            logger.LogDebug($"New location type for visitor is {type}");

            string newLocationName = null;

            if (type.Equals(previousLocation?.LocationType))
            {
                logger.LogInformation($"Getting new location with preferred type {type}");
                newLocationName = GetNewClosestToLocation(visitor, previousLocation);
            }

            switch (type)
            {
                case LocationType.FAIRYTALE:
                    FairyTale fairyTale = newLocationName == null ? fairyTaleControl.GetRandom() : fairyTaleControl.FindFairyTaleByName(newLocationName);
                    logger.LogInformation($"Walking to fairy tale {fairyTale.Name}");
                    visitor.TargetLocation = fairyTale;
                    break;
                case LocationType.RIDE:
                    Ride ride = newLocationName == null ? rideControl.GetRandom() : rideControl.FindRideByName(newLocationName);
                    logger.LogInformation($"Walking to ride {ride.Name}");
                    visitor.TargetLocation = ride;
                    break;
                case LocationType.STAND:
                    Ride stand = rideControl.GetRandom();
                    logger.LogInformation($"Temp: Walking to ride {stand.Name}");
                    visitor.TargetLocation = stand;
                    break;
            }
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
            return FilterNotVisited(visitor, location.DistanceToOthers.Values.ToList()).First();
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

        public void SetNewLocation(Visitor visitor);
    }
}
