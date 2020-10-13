using Confluent.Kafka;
using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace DddEfteling.FairyTales.Boundaries
{
    public class EventConsumer: KafkaConsumer, IEventConsumer
    {

        private readonly IFairyTaleControl fairyTaleControl;

        public EventConsumer(IFairyTaleControl fairyTaleControl) : base("domainEvents", "192.168.1.247:9092", "fairytales")
        {
            
            this.fairyTaleControl = fairyTaleControl;
        }

        protected override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.ArrivedAtFairyTale))
            {
                if (incomingEvent.Payload.TryGetValue("Visitor", out string visitorGuid))
                {
                    this.fairyTaleControl.HandleVisitorArrivingAtFairyTale(Guid.Parse(visitorGuid));
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
