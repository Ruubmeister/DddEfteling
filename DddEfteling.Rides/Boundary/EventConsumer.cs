using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;

namespace DddEfteling.Rides.Boundaries
{
    public class EventConsumer: KafkaConsumer, IEventConsumer
    {

        private readonly IRideControl rideControl;

        public EventConsumer(IRideControl rideControl) : base("domainEvents", "192.168.1.247:9092", "fairytales")
        {
            
            this.rideControl = rideControl;
        }

        protected override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.StepInRideLine))
            {
                if (incomingEvent.Payload.TryGetValue("Visitor", out string visitorGuid) &&
                    incomingEvent.Payload.TryGetValue("Ride", out string rideGuid))
                {
                    this.rideControl.HandleVisitorSteppingInRideLine(Guid.Parse(visitorGuid), Guid.Parse(rideGuid));
                }
            }
            else if (incomingEvent.Type.Equals(EventType.EmployeeChangedWorkplace))
            {
                if(incomingEvent.Payload.TryGetValue("Workplace", out string workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Employee", out string employeeString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out string workplaceSkill))
                {
                    Enum.TryParse(workplaceSkill, out WorkplaceSkill skill);
                    rideControl.HandleEmployeeChangedWorkplace(JsonConvert.DeserializeObject<WorkplaceDto>(workplaceString),
                        Guid.Parse(employeeString), skill);
                }
            }
            else if (incomingEvent.Type.Equals(EventType.StatusChanged) && incomingEvent.Source.Equals(EventSource.Park))
            {
                if (incomingEvent.Payload.TryGetValue("Status", out string statusString))
                {
                    if (statusString.ToLower().Equals("open"))
                    {
                        this.rideControl.OpenRides();
                    }else if (statusString.ToLower().Equals("closed"))
                    {
                        this.rideControl.CloseRides();
                    }
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }

}
