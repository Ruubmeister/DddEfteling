using Confluent.Kafka;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DddEfteling.Visitors.Boundaries
{
    public class EventConsumer: KafkaConsumer, IEventConsumer
    {

        private readonly IVisitorControl visitorControl;

        public EventConsumer(IVisitorControl visitorControl): base("domainEvents", "192.168.1.247:9092", "visitors")
        {
            
            this.visitorControl = visitorControl;
        }

        protected override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.VisitorsUnboarded))
            {
                List<Guid> visitors = JsonConvert.DeserializeObject<List<Guid>>(incomingEvent.Payload.Where(item => item.Key.Equals("Visitors")).First().Value);
                DateTime dateTime = JsonConvert.DeserializeObject<DateTime>(incomingEvent.Payload.Where(item => item.Key.Equals("DateTime")).First().Value);
                foreach (Guid visitorGuid in visitors)
                {
                    Visitor visitor = visitorControl.GetVisitor(visitorGuid);
                    visitor.TargetLocation = null;
                    visitorControl.AddIdleVisitor(visitorGuid, dateTime);
                }
            }
            else if (incomingEvent.Type.Equals(EventType.WatchingFairyTale))
            {
                if (incomingEvent.Payload.TryGetValue("Visitor", out string visitorGuid) &&
                incomingEvent.Payload.TryGetValue("EndDateTime", out string endDateTime))
                {
                    this.visitorControl.AddBusyVisitor(Guid.Parse(visitorGuid), DateTime.Parse(endDateTime));
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
