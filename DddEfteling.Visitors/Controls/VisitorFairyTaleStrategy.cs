using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorFairyTaleStrategy: IVisitorLocationStrategy
    {
        private readonly IEventProducer eventProducer;
        private readonly IFairyTaleClient fairyTaleClient;

        public VisitorFairyTaleStrategy(IEventProducer eventProducer, IFairyTaleClient fairyTaleClient)
        {
            this.eventProducer = eventProducer;
            this.fairyTaleClient = fairyTaleClient;
        }

        public void StartLocationActivity(Visitor visitor)
        {
            
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() }
            };
            
            eventPayload.Add("FairyTale", visitor.TargetLocation.Guid.ToString());
            Event watchingFairyTale = new Event(EventType.ArrivedAtFairyTale, EventSource.Visitor, eventPayload);
            eventProducer.Produce(watchingFairyTale);
            visitor.DoActivity(visitor.TargetLocation);
        }

        public void SetNewLocation(Visitor visitor)
        {
            ILocationDto previousLocation = visitor.GetLastLocation();

            visitor.TargetLocation = null;

            if (previousLocation is {LocationType: LocationType.FAIRYTALE})
            {
                visitor.TargetLocation = fairyTaleClient.GetNewFairyTaleLocation(previousLocation.Guid,
                    visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
            }

            visitor.TargetLocation ??= fairyTaleClient.GetRandomFairyTale();
        }
    }
}