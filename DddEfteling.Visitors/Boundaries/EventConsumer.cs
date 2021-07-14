using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DddEfteling.Visitors.Boundaries
{
    public class EventConsumer : KafkaConsumer, IEventConsumer
    {

        private readonly IVisitorControl visitorControl;

        public EventConsumer(IVisitorControl visitorControl, IConfiguration configuration) :
            base("domainEvents", configuration["KafkaBroker"], "visitors")
        {

            this.visitorControl = visitorControl;
        }

        public override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.VisitorsUnboarded))
            {
                List<Guid> visitors = JsonConvert.DeserializeObject<List<Guid>>(incomingEvent.Payload.First(item => item.Key.Equals("Visitors")).Value);
                DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(incomingEvent.Payload.First(item => item.Key.Equals("DateTime")).Value);
                foreach (Guid visitorGuid in visitors)
                {
                    Visitor visitor = visitorControl.GetVisitor(visitorGuid);
                    visitor.TargetLocation = null;
                    visitorControl.UpdateVisitorAvailabilityAt(visitorGuid, dateTime);
                }
            }
            else if (incomingEvent.Type.Equals(EventType.WatchingFairyTale) && incomingEvent.Payload.TryGetValue("Visitor", out string visitorGuid) &&
                incomingEvent.Payload.TryGetValue("EndDateTime", out string endDateTime))
            {
                Guid visitorGuidObject = Guid.Parse(visitorGuid);
                this.visitorControl.RemoveVisitorTargetLocation(visitorGuidObject);
                this.visitorControl.UpdateVisitorAvailabilityAt(visitorGuidObject, DateTime.Parse(endDateTime));
            }
            else if (incomingEvent.Type.Equals(EventType.WaitingForOrder) &&
                     incomingEvent.Payload.TryGetValue("Visitor", out string waitingForOrderVisitorGuid) &&
                     incomingEvent.Payload.TryGetValue("Ticket", out string ticket))
            {
                this.visitorControl.AddVisitorWaitingForOrder(ticket, Guid.Parse(waitingForOrderVisitorGuid));
            }
            else if (incomingEvent.Type.Equals(EventType.OrderReady) && incomingEvent.Payload.TryGetValue("Order", out string order))
            {
                this.visitorControl.NotifyOrderReady(order);
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
