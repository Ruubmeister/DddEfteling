using Confluent.Kafka;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Controls;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace DddEfteling.Visitors.Boundaries
{
    public class EventConsumer: KafkaConsumer, IEventConsumer
    {

        private readonly IVisitorControl visitorControl;

        public EventConsumer(IVisitorControl visitorControl): base("events", "192.168.1.247:9092", "visitors")
        {
            
            this.visitorControl = visitorControl;
        }

        protected override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.VisitorsUnboarded))
            {
                // Here we wanna do some stuff with unboarded visitors
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
