using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Visitors.Boundaries
{
    public class EventConsumer : KafkaConsumer, IEventConsumer
    {

        private readonly IVisitorControl visitorControl;

        public EventConsumer(IVisitorControl visitorControl) : base("domainEvents", "192.168.1.247:9092", "visitors")
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
                    visitorControl.AddIdleVisitor(visitorGuid, dateTime);
                }
            }
            else if (incomingEvent.Type.Equals(EventType.WatchingFairyTale) && incomingEvent.Payload.TryGetValue("Visitor", out string visitorGuid) &&
                incomingEvent.Payload.TryGetValue("EndDateTime", out string endDateTime))
            {
                this.visitorControl.AddBusyVisitor(Guid.Parse(visitorGuid), DateTime.Parse(endDateTime));
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
