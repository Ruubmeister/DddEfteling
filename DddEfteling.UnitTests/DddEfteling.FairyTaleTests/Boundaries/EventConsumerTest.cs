using DddEfteling.FairyTales.Boundaries;
using DddEfteling.FairyTales.Controls;
using DddEfteling.Shared.Entities;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.FairyTaleTests.Boundaries
{
    public class EventConsumerTest
    {
        private readonly EventConsumer eventConsumer;
        private readonly Mock<IFairyTaleControl> fairyTaleMock;

        public EventConsumerTest()
        {
            this.fairyTaleMock = new Mock<IFairyTaleControl>();
            this.fairyTaleMock.Setup(control => control.HandleVisitorArrivingAtFairyTale(It.Ref<Guid>.IsAny));

            this.eventConsumer = new EventConsumer(this.fairyTaleMock.Object);
        }

        [Fact]
        public void HandleMessage_ExpectArrivedAtFairyTaleEvent_CallsControlFunction()
        {
            Guid guid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", guid.ToString() } };
            Event incomingEvent = new Event(EventType.ArrivedAtFairyTale, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            fairyTaleMock.Verify(control => control.HandleVisitorArrivingAtFairyTale(guid), Times.Once);

        }

        [Fact]
        public void HandleMessage_ExpectUnknownEvent_NoCallToControl()
        {
            Guid guid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", guid.ToString() } };
            Event incomingEvent = new Event(EventType.Idle, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            fairyTaleMock.Verify(control => control.HandleVisitorArrivingAtFairyTale(It.IsAny<Guid>()), Times.Never);

        }
    }
}
