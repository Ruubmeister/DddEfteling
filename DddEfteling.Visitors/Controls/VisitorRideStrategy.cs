using System.Collections.Generic;
using System.Linq;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorRideStrategy
    {
        private readonly IEventProducer eventProducer;
        private readonly IRideClient rideClient;

        public VisitorRideStrategy(IEventProducer eventProducer, IRideClient rideClient)
        {
            this.eventProducer = eventProducer;
            this.rideClient = rideClient;
        }

        public void StartLocationActivity(Visitor visitor)
        {
            var eventPayload = new Dictionary<string, string>
            {
                {"Visitor", visitor.Guid.ToString() }
            };
            
            eventPayload.Add("Ride", visitor.TargetLocation.Guid.ToString());
            Event steppingIntoRide = new Event(EventType.StepInRideLine, EventSource.Visitor, eventPayload);
            eventProducer.Produce(steppingIntoRide);
            
            visitor.DoActivity(visitor.TargetLocation);
        }

        public void SetNewLocation(Visitor visitor)
        {
            ILocationDto previousLocation = visitor.GetLastLocation();

            visitor.TargetLocation = null;

            if (previousLocation is {LocationType: LocationType.RIDE})
            {
                visitor.TargetLocation = rideClient.GetNextLocation(previousLocation.Guid,
                    visitor.VisitedLocations.Values.Select(location => location.Guid).ToList());
            }

            visitor.TargetLocation ??= rideClient.GetRandomRide();
        }
    }
}