using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorControl : IVisitorControl
    {
        private readonly IMediator mediator;
        private readonly IStandClient standClient;
        private readonly ILogger<VisitorControl> logger;
        private readonly IVisitorRepository repository;
        private readonly ILocationTypeStrategy locationTypeStrategy;

        public readonly ConcurrentDictionary<string, Guid> VisitorsWaitingForOrder = new ConcurrentDictionary<string, Guid>();
        private readonly IVisitorMovementService visitorMovementService;


        public VisitorControl(IMediator mediator, ILogger<VisitorControl> logger, IStandClient standClient,
            IVisitorRepository repository, IVisitorMovementService visitorMovementService, 
            ILocationTypeStrategy locationTypeStrategy)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.standClient = standClient;
            this.repository = repository;
            this.visitorMovementService = visitorMovementService;
            this.locationTypeStrategy = locationTypeStrategy;
        }

        public void HandleIdleVisitors()
        {
            List<Visitor> idleVisitors = repository.IdleVisitors();

            if (!idleVisitors.Any())
            {
                logger.LogInformation("No idle visitors found");
                return;
            }

            foreach (var idleVisitor in idleVisitors)
            {
                SetLocation(idleVisitor);

                if (idleVisitor.TargetLocation == null)
                {
                    logger.LogInformation("Visitor with guid {VisitorGuid} has issue with assigning location", 
                        idleVisitor.Guid);
                    NotifyIdleVisitor(idleVisitor.Guid);
                    continue;
                }
                
                visitorMovementService.SetNextStepDistance(idleVisitor);

                if (VisitorMovementService.IsInLocationRange(idleVisitor))
                {
                    logger.LogDebug("Visitor {VisitorGuid} arrived at location {LocationName}", 
                        idleVisitor.Guid, idleVisitor.TargetLocation.Name);
                    idleVisitor.LocationStrategy.StartLocationActivity(idleVisitor);
                    idleVisitor.AvailableAt = null;
                }
                else
                {
                    logger.LogDebug("Visitor {VisitorGuid} walking to location {LocationName}", 
                        idleVisitor.Guid, idleVisitor.TargetLocation.Name);
                    idleVisitor.WalkToDestination();
                    this.NotifyIdleVisitor(idleVisitor.Guid);
                }
            }
        }

        private void SetLocation(Visitor visitor)
        {
            if (visitor.TargetLocation != null)
            {
                return;
            }
            
            ILocationDto previousLocation = visitor.GetLastLocation();
            LocationType type = visitor.GetLocationType(previousLocation?.LocationType);

            IVisitorLocationStrategy strategy = locationTypeStrategy.GetStrategy(type);
            visitor.LocationStrategy = strategy;
            strategy.SetNewLocation(visitor);
        }
        
        public void NotifyOrderReady(string guid)
        {
            if (this.VisitorsWaitingForOrder.TryGetValue(guid, out Guid visitorGuid))
            {
                Visitor visitor = GetVisitor(visitorGuid);
                visitor.PickUpOrder(standClient, guid);
                // Todo: Fix hardcoded values 
                DateTime timeWhenConsumed = DateTime.Now.AddMinutes(2);
                Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "DateTime", timeWhenConsumed }
                };

                this.mediator.Publish(new VisitorEvent(EventType.Idle, visitor.Guid, payload));
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

        public void UpdateVisitorAvailabilityAt(Guid visitorGuid, DateTime dateTime)
        {
            Visitor visitor = repository.GetVisitor(visitorGuid);
            if (visitor != null)
            {
                visitor.AvailableAt = dateTime;
            }
        }

        public List<Visitor> All()
        {
            return repository.All();
        }

        public Visitor GetVisitor(Guid guid)
        {
            return repository.GetVisitor(guid);
        }

        public void RemoveVisitorTargetLocation(Guid guid)
        {
            Visitor visitor = GetVisitor(guid);
            visitor.TargetLocation = null;
        }

        public void AddVisitors(int number)
        {
            var visitors = this.repository.AddVisitors(number);
            foreach(Visitor visitor in visitors)
            {
                UpdateVisitorAvailabilityAt(visitor.Guid, DateTime.Now);
            }
        }

        public void AddVisitorWaitingForOrder(string ticket, Guid guid)
        {
            VisitorsWaitingForOrder.TryAdd(ticket, guid);
        }
    }

    public interface IVisitorControl
    {
        public void AddVisitors(int number);
        public List<Visitor> All();

        public void UpdateVisitorAvailabilityAt(Guid visitorGuid, DateTime dateTime);

        public void HandleIdleVisitors();

        public Visitor GetVisitor(Guid guid);

        public void NotifyOrderReady(string guid);

        public void AddVisitorWaitingForOrder(string ticket, Guid guid);

        public void RemoveVisitorTargetLocation(Guid guid);
    }
}