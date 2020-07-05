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
        private Random random = new Random();
        private IMediator mediator;
        private IOptions<VisitorSettings> visitorSettings;
        private Coordinate startCoordinate = new Coordinate(51.649175, 5.045545);
        private ILogger<VisitorControl> logger;
        private IFairyTaleControl fairyTaleControl;
        private IRideControl rideControl;
        private IStandControl standControl;

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
                  LocationType? previousLocationType = null;
                  ILocation previousLocation = null;
                  string newLocationName = null;

                  while (true)
                  {
                      LocationType type = visitor.GetLocationType(previousLocationType);

                      if (type.Equals(previousLocationType))
                      {
                          newLocationName = GetNewClosestToLocation(visitor, previousLocation);
                      }

                      switch (type)
                      {
                          case LocationType.FAIRYTALE:
                              FairyTale fairyTale = newLocationName == null ? fairyTaleControl.GetRandom() : fairyTaleControl.FindFairyTaleByName(newLocationName);
                              visitor.WalkTo(fairyTale.Coordinates);
                              visitor.WatchFairyTale(fairyTale);
                              previousLocation = fairyTale;
                              break;
                          case LocationType.RIDE:
                              Ride ride = newLocationName == null ? rideControl.GetRandom() :rideControl.FindRideByName(newLocationName);
                              visitor.WalkTo(ride.Coordinates);
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
