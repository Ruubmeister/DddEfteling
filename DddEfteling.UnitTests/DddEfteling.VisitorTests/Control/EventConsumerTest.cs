using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Boundaries;
using DddEfteling.Visitors.Controls;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.VisitorTests.Boundaries
{
    public class EventConsumerTest
    {
        private readonly EventConsumer eventConsumer;
        private readonly Mock<IVisitorControl> visitorMock;

        public EventConsumerTest()
        {
            this.visitorMock = new Mock<IVisitorControl>();
            this.visitorMock.Setup(control => control.GetVisitor(It.IsAny<Guid>())).Returns(new Visitors.Entities.Visitor());
            this.visitorMock.Setup(control => control.AddIdleVisitor(It.IsAny<Guid>(), It.Ref<DateTime>.IsAny));
            this.visitorMock.Setup(control => control.AddBusyVisitor(It.IsAny<Guid>(), It.Ref<DateTime>.IsAny));

            this.eventConsumer = new EventConsumer(this.visitorMock.Object);
        }

        [Fact]
        public void HandleMessage_ExpectVisitorsUnboardedEvent_CallsControlFunction()
        {
            List<Guid> idleVisitors = new List<Guid>() { { Guid.NewGuid() }, { Guid.NewGuid() } };
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitors", JsonConvert.SerializeObject(idleVisitors) },
                { "DateTime", JsonConvert.SerializeObject(DateTime.Now) } };
            Event incomingEvent = new Event(EventType.VisitorsUnboarded, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            visitorMock.Verify(control => control.GetVisitor(It.IsAny<Guid>()), Times.Exactly(2));
            visitorMock.Verify(control => control.AddIdleVisitor(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Exactly(2));

        }

        [Fact]
        public void HandleMessage_ExpectUnknownEvent_NoCallToControl()
        {
            Guid guid = Guid.NewGuid();
            Dictionary<string, string> payload = new Dictionary<string, string>() { { "Visitor", guid.ToString() } };
            Event incomingEvent = new Event(EventType.StatusChanged, EventSource.Visitor, payload);
            this.eventConsumer.HandleMessage(JsonConvert.SerializeObject(incomingEvent));

            visitorMock.Verify(control => control.AddIdleVisitor(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
            visitorMock.Verify(control => control.AddBusyVisitor(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);

        }
    }
}
