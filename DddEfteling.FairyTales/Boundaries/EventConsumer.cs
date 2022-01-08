using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;

namespace DddEfteling.FairyTales.Boundaries
{
    public class EventConsumer : KafkaConsumer, IEventConsumer
    {

        private readonly IFairyTaleControl fairyTaleControl;

        public EventConsumer(IFairyTaleControl fairyTaleControl, IConfiguration configuration) :
            base("domainEvents", configuration["KafkaBroker"], "fairytales")
        {

            this.fairyTaleControl = fairyTaleControl;
        }

        public override void HandleMessage(string incomingMessage)
        {

            var incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent is not null && incomingEvent.Type.Equals(EventType.ArrivedAtFairyTale)
                                          && incomingEvent.Payload.TryGetValue("Visitor", out var visitorGuid))
            {
                fairyTaleControl.HandleVisitorArrivingAtFairyTale(Guid.Parse(visitorGuid));
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
