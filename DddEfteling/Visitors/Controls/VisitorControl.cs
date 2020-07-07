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
                Visitor visitor = new Visitor(DateTime.Now, 1.55, startCoordinate, random, mediator, visitorSettings);
                Visitors.Add(visitor);
                KickOffVisitor(visitor);
            }
        }

        private async void KickOffVisitor(Visitor visitor)
        {
            _ = Task.Run(async () =>
            {
                logger.LogDebug($"Kicking off visitor {visitor.Guid}");
                LocationType? previousLocationType = null;
                ILocation previousLocation = null;
                string newLocationName = null;

                while (true)
                {
                    LocationType type = visitor.GetLocationType(previousLocationType);
                    logger.LogDebug($"New location type for visitor is {type}");

                    if (type.Equals(previousLocationType))
                    {
                        logger.LogDebug($"Getting new location with preferred type {type}");
                        newLocationName = GetNewClosestToLocation(visitor, previousLocation);
                    }

                    switch (type)
                    {
                        case LocationType.FAIRYTALE:
                            FairyTale fairyTale = newLocationName == null ? fairyTaleControl.GetRandom() : fairyTaleControl.FindFairyTaleByName(newLocationName);
                            logger.LogDebug($"Walking to fairy tale {fairyTale.Name}");
                            visitor.WalkTo(fairyTale.Coordinates);
                            logger.LogDebug($"Watching fairy tale {fairyTale.Name}");
                            visitor.WatchFairyTale(fairyTale);
                            previousLocation = fairyTale;
                            break;
                        case LocationType.RIDE:
                            Ride ride = newLocationName == null ? rideControl.GetRandom() : rideControl.FindRideByName(newLocationName);
                            logger.LogDebug($"Walking to ride ride {ride.Name}");
                            visitor.WalkTo(ride.Coordinates);
                            logger.LogDebug($"Going in ride {ride.Name}");
                            visitor.StepInRide(ride);
                            previousLocation = ride;
                            break;
                    }
                }
            });
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
    }

    public interface IVisitorControl
    {
        public void AddVisitors(int number);
        public List<Visitor> All();
    }
}
