using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DddEfteling.ParkTests.Boundaries
{
    public class EventConsumerTest
    {
        private readonly EventConsumer eventConsumer;
        private readonly Mock<IEmployeeControl> employeeMock;
        
        IConfigurationRoot Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"KafkaBroker", "localhost"}
            })
            .Build();

        public EventConsumerTest()
        {
            this.employeeMock = new Mock<IEmployeeControl>();
            this.employeeMock.Setup(control => control.AssignEmployee(It.Ref<WorkplaceDto>.IsAny, It.Ref<WorkplaceSkill>.IsAny));

            this.eventConsumer = new EventConsumer(this.employeeMock.Object, Configuration);
        }

        [Fact]
        public void HandleMessage_ExpectRequestEmployeeEvent_CallsControlFunction()
        {
            WorkplaceDto workplaceDto = new WorkplaceDto(Guid.NewGuid(), LocationType.RIDE);
            WorkplaceSkill skill = WorkplaceSkill.Control;
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Workplace", JsonConvert.SerializeObject(workplaceDto) }, { "Skill", skill.ToString() } };
            Event incomingEvent = new Event(EventType.RequestEmployee, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            employeeMock.Verify(control => control.AssignEmployee(It.IsAny<WorkplaceDto>(), It.IsAny<WorkplaceSkill>()), Times.Once);

        }

        [Fact]
        public void HandleMessage_ExpectUnknownEvent_NoCallToControl()
        {
            Guid guid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", guid.ToString() } };
            Event incomingEvent = new Event(EventType.Idle, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            employeeMock.Verify(control => control.AssignEmployee(It.IsAny<WorkplaceDto>(), It.IsAny<WorkplaceSkill>()), Times.Never);

        }
    }
}
