using DddEfteling.Park.Controls;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;
using Microsoft.Extensions.Configuration;

namespace DddEfteling.Park.Boundaries
{
    public class EventConsumer : KafkaConsumer, IEventConsumer
    {

        private readonly IEmployeeControl employeeControl;

        public EventConsumer(IEmployeeControl employeeControl, IConfiguration configuration) :
            base("domainEvents", configuration["KafkaBroker"], "park")
        {

            this.employeeControl = employeeControl;
        }

        public override void HandleMessage(string incomingMessage)
        {

            var incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent is not null  && incomingEvent.Type.Equals(EventType.RequestEmployee) && 
                incomingEvent.Payload.TryGetValue("Workplace", out var workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out var skillString))
            {
                WorkplaceDto workplaceDto = JsonConvert.DeserializeObject<WorkplaceDto>(workplaceString);
                if (Enum.TryParse(skillString, out WorkplaceSkill skill))
                {
                    employeeControl.AssignEmployee(workplaceDto, skill);
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
