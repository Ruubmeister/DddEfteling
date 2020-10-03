using DddEfteling.Park.Controls;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
using Newtonsoft.Json;
using System;

namespace DddEfteling.Park.Boundaries
{
    public class EventConsumer: KafkaConsumer, IEventConsumer
    {

        private readonly IEmployeeControl employeeControl;

        public EventConsumer(IEmployeeControl employeeControl) : base("events", "192.168.1.247:9092", "fairytales")
        {
            
            this.employeeControl = employeeControl;
        }

        protected override void HandleMessage(string incomingMessage)
        {

            Event incomingEvent = JsonConvert.DeserializeObject<Event>(incomingMessage);

            if (incomingEvent.Type.Equals(EventType.RequestEmployee))
            {
                if (incomingEvent.Payload.TryGetValue("Workplace", out string workplaceString) &&
                    incomingEvent.Payload.TryGetValue("Skill", out string skillString))
                {
                    WorkplaceDto workplaceDto = JsonConvert.DeserializeObject<WorkplaceDto>(workplaceString);
                    Enum.TryParse(skillString, out WorkplaceSkill skill);
                    this.employeeControl.AssignEmployee(workplaceDto, skill);
                }
            }
        }
    }

    public interface IEventConsumer
    {
        public void Listen();
    }
}
