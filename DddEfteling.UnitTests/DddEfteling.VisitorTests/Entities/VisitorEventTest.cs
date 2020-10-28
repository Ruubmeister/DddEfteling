using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DddEfteling.VisitorTests.Entities
{
    public class VisitorEventTest
    {
        [Fact]
        public void Constructors_ConstructVisitorEvent_ExpectCorrectVisitorEvent()
        {
            Guid guid = Guid.NewGuid();
            VisitorEvent visitorEvent1 = new VisitorEvent(EventType.Idle, guid);

            Assert.Equal(guid, visitorEvent1.VisitorGuid);
            Assert.Equal(EventType.Idle, visitorEvent1.Type);

            VisitorEvent visitorEvent2 = new VisitorEvent(EventType.Idle, guid, new Dictionary<string, object>() { { "payload key", "payload value" } });

            Assert.Equal(guid, visitorEvent2.VisitorGuid);
            Assert.Equal(EventType.Idle, visitorEvent2.Type);
            Assert.NotEmpty(visitorEvent2.Payload);
            Assert.True(visitorEvent2.Payload.ContainsKey("payload key"));
            Assert.Equal("payload value", visitorEvent2.Payload.First(kv => kv.Key.Equals("payload key")).Value);
        }
    }
}
