using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Entities;
using Geolocation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorControl : IVisitorControl
    {
        private readonly Random random = new Random();
        private readonly IMediator mediator;
        private readonly IRideClient rideClient;
        private readonly IFairyTaleClient fairyTaleClient;
        private readonly IOptions<VisitorSettings> visitorSettings;
        private readonly Coordinate startCoordinate = new Coordinate(51.649175, 5.045545);
        private readonly ILogger<VisitorControl> logger;
        private readonly IEventProducer eventProducer;

        public readonly Dictionary<Guid, DateTime> IdleVisitors = new Dictionary<Guid, DateTime>();
        public readonly ConcurrentDictionary<Guid, DateTime> BusyVisitors = new ConcurrentDictionary<Guid, DateTime>();

        private ConcurrentBag<Visitor> Visitors { get; } = new ConcurrentBag<Visitor>();

        public VisitorControl(IMediator mediator, IOptions<VisitorSettings> settings, ILogger<VisitorControl> logger,
            IRideClient rideClient, IFairyTaleClient fairyTaleClient, IEventProducer eventProducer)
        {
            this.mediator = mediator;
            this.visitorSettings = settings;
            this.logger = logger;
            this.rideClient = rideClient;
            this.fairyTaleClient = fairyTaleClient;
            this.eventProducer = eventProducer;
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
                Visitor visitor = Visitors.First(visitor => visitor.Guid.Equals(visitorAtTime.Key));
                if (visitor.TargetLocation == null)
                {
                    this.SetNewLocation(visitor);
                }

                if (visitor.TargetLocation == null)
                {
                    this.NotifyIdleVisitor(visitor.Guid);
                    continue;
                }

                double step = (double)random.Next(50, 150) / 100;
                TimeSpan timeIdle = DateTime.Now - visitorAtTime.Value;

                double correctedStep = timeIdle.TotalSeconds * step;

                if (CoordinateExtensions.IsInRange(visitor.CurrentLocation, visitor.TargetLocation.Coordinates, correctedStep))
                {
                    var eventPayload = new Dictionary<string, string>
                        {
                            {"Visitor", visitor.Guid.ToString() }
                        };

                    if (visitor.TargetLocation.LocationType.Equals(LocationType.RIDE))
                    {
                        this.logger.LogInformation($"Visitor {visitor.Guid} stepping into ride {visitor.TargetLocation.Name}");
                        eventPayload.Add("Ride", visitor.TargetLocation.Guid.ToString());
                        Event stepInRideLine = new Event(EventType.StepInRideLine, EventSource.Visitor, eventPayload);
                        eventProducer.Produce(stepInRideLine);
                    }
                    else if (visitor.TargetLocation.LocationType.Equals(LocationType.FAIRYTALE))
                    {
                        this.logger.LogInformation($"Visitor {visitor.Guid} watching fairytale {visitor.TargetLocation.Name}");
                        eventPayload.Add("FairyTale", visitor.TargetLocation.Guid.ToString());
                        Event watchingFairyTale = new Event(EventType.ArrivedAtFairyTale, EventSource.Visitor, eventPayload);
                        eventProducer.Produce(watchingFairyTale);
                    }
                }
                else
                {
                    visitor.WalkToDestination(correctedStep);

                    this.NotifyIdleVisitor(visitor.Guid);
                }
            }
        }

        public void NotifyIdleVisitor(Guid guid)
        {
            Dictionary<string, object> payload = new Dictionary<string, object>
                    {
                        { "DateTime", DateTime.Now }
                    };

            this.mediator.Publish(new VisitorEvent(EventType.Idle, guid, payload));
        }

        public void HandleBusyVisitors()
        {
            if (this.BusyVisitors.Any())
            {
                foreach (KeyValuePair<Guid, DateTime> visitorBusyTime in this.BusyVisitors)
                {
                    if (visitorBusyTime.Value <= DateTime.Now)
                    {
                        Visitor visitor = this.GetVisitor(visitorBusyTime.Key);
                        visitor.TargetLocation = null;
                        this.NotifyIdleVisitor(visitor.Guid);
                        this.BusyVisitors.TryRemove(visitor.Guid, out _);
                    }
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
            ILocationDto previousLocation = visitor.GetLastLocation();

            LocationType type = visitor.GetLocationType(previousLocation?.LocationType);
            logger.LogDebug($"New location type for visitor is {type}");

            switch (type)
            {
                case LocationType.FAIRYTALE:
                    FairyTaleDto fairyTale = null;

                    if (previousLocation != null && type.Equals(previousLocation?.LocationType))
                    {
                        fairyTale = fairyTaleClient.GetNearestFairyTale(previousLocation.Guid,
                        visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
                    }

                    if (fairyTale == null)
                    {
                        fairyTale = fairyTaleClient.GetRandomFairyTale();
                    }

                    logger.LogInformation($"Walking to fairy tale {fairyTale?.Name}");
                    visitor.TargetLocation = fairyTale;
                    break;
                case LocationType.RIDE:

                    RideDto ride = null;

                    if (previousLocation != null && type.Equals(previousLocation?.LocationType))
                    {
                        ride = rideClient.GetNearestRide(previousLocation.Guid,
                        visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
                    }

                    if (ride == null)
                    {
                        ride = rideClient.GetRandomRide();
                    }

                    logger.LogInformation($"Walking to ride {ride?.Name}");
                    visitor.TargetLocation = ride;
                    break;
                case LocationType.STAND:

                    RideDto stand = null;

                    if (previousLocation != null && type.Equals(previousLocation?.LocationType))
                    {
                        stand = rideClient.GetNearestRide(previousLocation.Guid,
                        visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
                    }

                    if (stand == null)
                    {
                        stand = rideClient.GetRandomRide();
                    }

                    logger.LogInformation($"Temp: Walking to ride {stand?.Name}");
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
                    FairyTaleDto tale = fairyTaleClient.GetRandomFairyTale();
                    visitor.TargetLocation = tale;
                    break;
                case LocationType.RIDE:
                    RideDto ride = rideClient.GetRandomRide();
                    visitor.TargetLocation = ride;
                    break;
            }

            Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "DateTime", DateTime.Now }
                };

            this.mediator.Publish(new VisitorEvent(EventType.Idle, visitor.Guid, payload));

        }

        public void AddIdleVisitor(Guid visitorGuid, DateTime dateTime)
        {
            this.IdleVisitors.Add(visitorGuid, dateTime);
        }

        public void AddBusyVisitor(Guid visitorGuid, DateTime dateTime)
        {
            if (!this.BusyVisitors.ContainsKey(visitorGuid))
            {
                this.BusyVisitors.TryAdd(visitorGuid, dateTime);
            }
        }

        public Visitor GetVisitor(Guid guid)
        {
            return this.Visitors.First(visitor => visitor.Guid.Equals(guid));
        }
    }

    public interface IVisitorControl
    {
        public void AddVisitors(int number);
        public List<Visitor> All();

        public void AddIdleVisitor(Guid visitorGuid, DateTime dateTime);

        public void HandleBusyVisitors();

        public void HandleIdleVisitors();

        public void SetNewLocation(Visitor visitor);

        public void AddBusyVisitor(Guid visitorGuid, DateTime dateTime);

        public Visitor GetVisitor(Guid guid);
    }
}
