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
                if (incomingEvent.Payload.TryGetValue("Workplace", out string workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Employee", out string employeeString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out string workplaceSkill))
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
                incomingEvent.Payload.TryGetValue("Status", out string statusString))
            {
                if (statusString.ToLower().Equals("open"))
                {
                    this.rideControl.OpenRides();
                }
                else if (statusString.ToLower().Equals("closed"))
                {
                    this.rideControl.CloseRides();
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }

}
