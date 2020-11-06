using DddEfteling.Rides.Boundaries;
using DddEfteling.Rides.Controls;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.RideTests.Boundaries
{
    public class EventConsumerTest
    {
        private readonly EventConsumer eventConsumer;
        private readonly Mock<IRideControl> rideMock;

        public EventConsumerTest()
        {
            this.rideMock = new Mock<IRideControl>();
            this.rideMock.Setup(control => control.HandleVisitorSteppingInRideLine(It.Ref<Guid>.IsAny, It.Ref<Guid>.IsAny));
            this.rideMock.Setup(control => control.HandleEmployeeChangedWorkplace(It.Ref<WorkplaceDto>.IsAny, It.Ref<Guid>.IsAny, It.Ref<WorkplaceSkill>.IsAny));
            this.rideMock.Setup(control => control.OpenRides());
            this.rideMock.Setup(control => control.CloseRides());

            this.eventConsumer = new EventConsumer(this.rideMock.Object);
        }

        [Fact]
        public void HandleMessage_ExpectVisitorSteppingIntoLineEvent_CallsControlFunction()
        {
            Guid visitorGuid = Guid.NewGuid();
            Guid rideGuid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", visitorGuid.ToString() }, { "Ride", rideGuid.ToString() } };
            Event incomingEvent = new Event(EventType.StepInRideLine, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            rideMock.Verify(control => control.HandleVisitorSteppingInRideLine(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);

        }

        [Fact]
        public void HandleMessage_ExpectEmployeeChangedWorkplaceEvent_CallsControlFunction()
        {
            Guid employeeGuid = Guid.NewGuid();
            WorkplaceDto workplaceDto = new WorkplaceDto(Guid.NewGuid(), LocationType.RIDE);
            WorkplaceSkill skill = WorkplaceSkill.Engineer;

            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Employee", employeeGuid.ToString() },
                { "Workplace", JsonConvert.SerializeObject(workplaceDto) }, { "Skill", skill.ToString() } };

            Event incomingEvent = new Event(EventType.EmployeeChangedWorkplace, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            rideMock.Verify(control => control.HandleEmployeeChangedWorkplace(It.IsAny<WorkplaceDto>(), It.IsAny<Guid>(), It.IsAny<WorkplaceSkill>()), Times.Once);

        }

        [Fact]
        public void HandleMessage_ExpectStatusChangedToOpenEvent_CallsControlFunction()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Status", "Open" } };

            Event incomingEvent = new Event(EventType.StatusChanged, EventSource.Park, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            rideMock.Verify(control => control.OpenRides(), Times.Once);
        }

        [Fact]
        public void HandleMessage_ExpectStatusChangedToCloseEvent_CallsControlFunction()
        {
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Status", "Closed" } };

            Event incomingEvent = new Event(EventType.StatusChanged, EventSource.Park, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            rideMock.Verify(control => control.CloseRides(), Times.Once);
        }

        [Fact]
        public void HandleMessage_ExpectUnknownEvent_NoCallToControl()
        {
            Guid guid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", guid.ToString() } };
            Event incomingEvent = new Event(EventType.Idle, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            rideMock.Verify(control => control.HandleVisitorSteppingInRideLine(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            rideMock.Verify(control => control.HandleEmployeeChangedWorkplace(It.IsAny<WorkplaceDto>(), It.IsAny<Guid>(), It.IsAny<WorkplaceSkill>()), Times.Never);
            rideMock.Verify(control => control.OpenRides(), Times.Never);
            rideMock.Verify(control => control.CloseRides(), Times.Never);

        }
    }
}
