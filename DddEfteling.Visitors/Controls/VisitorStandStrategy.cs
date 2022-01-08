using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorStandStrategy: IVisitorLocationStrategy
    {
        private readonly IEventProducer eventProducer;
        private readonly IStandClient standClient;

        public VisitorStandStrategy(IEventProducer eventProducer, IStandClient standClient)
        {
            this.eventProducer = eventProducer;
            this.standClient = standClient;
        }

        public void StartLocationActivity(Visitor visitor)
        {
            StandDto stand = standClient.GetStand(visitor.TargetLocation.Guid);
            string ticket = standClient.OrderDinner(stand.Guid, visitor.PickStandProducts(stand));
            
            visitor.DoActivity(visitor.TargetLocation);
            
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() },
                {"Ticket", ticket}
            };
            
            var waitingForOrder = new Event(EventType.WaitingForOrder, EventSource.Visitor, eventPayload);
            eventProducer.Produce(waitingForOrder);
        }

        public void SetNewLocation(Visitor visitor)
        {
            var previousLocation = visitor.GetLastLocation();

            visitor.TargetLocation = null;

            if (previousLocation is {LocationType: LocationType.RIDE})
            {
                visitor.TargetLocation = standClient.GetNewStandLocation(previousLocation.Guid,
                    visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
            }

            visitor.TargetLocation ??= standClient.GetRandomStand();
        }
    }
}