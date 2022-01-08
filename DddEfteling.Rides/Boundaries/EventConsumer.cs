using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;

namespace DddEfteling.Rides.Boundaries
{
    public class EventConsumer : KafkaConsumer, IEventConsumer
    {

        private readonly IRideControl rideControl;

        public EventConsumer(IRideControl rideControl, IConfiguration configuration) :
            base("domainEvents", configuration["KafkaBroker"], "rides")
        {

            this.rideControl = rideControl;
        }

        public override void HandleMessage(string incomingMessage)
        {

            var incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.StepInRideLine))
            {
                if (incomingEvent.Payload.TryGetValue("Visitor", out var visitorGuid) &&
                    incomingEvent.Payload.TryGetValue("Ride", out var rideGuid))
                {
                    rideControl.HandleVisitorSteppingInRideLine(Guid.Parse(visitorGuid), Guid.Parse(rideGuid));
                }
            }
            else if (incomingEvent.Type.Equals(EventType.EmployeeChangedWorkplace))
            {
                if (incomingEvent.Payload.TryGetValue("Workplace", out var workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Employee", out var employeeString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out var workplaceSkill))
                {
                    if (Enum.TryParse(workplaceSkill, out WorkplaceSkill skill))
                    {
                        rideControl.HandleEmployeeChangedWorkplace(
                            JsonConvert.DeserializeObject<WorkplaceDto>(workplaceString),
                            Guid.Parse(employeeString), skill);
                    }
                    
                }
            }
            else if (incomingEvent.Type.Equals(EventType.StatusChanged) && incomingEvent.Source.Equals(EventSource.Park) &&
                incomingEvent.Payload.TryGetValue("Status", out var statusString))
            {
                if (statusString.ToLower().Equals("open"))
                {
                    rideControl.OpenRides();
                }
                else if (statusString.ToLower().Equals("closed"))
                {
                    rideControl.CloseRides();
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }

}
