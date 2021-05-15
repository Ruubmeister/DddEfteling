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

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.RequestEmployee) && incomingEvent.Payload.TryGetValue("Workplace", out string workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out string skillString))
            {
                WorkplaceDto workplaceDto = JsonConvert.DeserializeObject<WorkplaceDto>(workplaceString);
                Enum.TryParse(skillString, out WorkplaceSkill skill);
                this.employeeControl.AssignEmployee(workplaceDto, skill);
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
