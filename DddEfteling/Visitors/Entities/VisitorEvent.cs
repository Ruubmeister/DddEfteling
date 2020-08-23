using DddEfteling.Park.Common.Entities;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Visitors.Entities
{
    public class VisitorEvent : Event
    {
        public VisitorEvent(EventType type, Guid visitorGuid) : base(type)
        {
            this.VisitorGuid = visitorGuid;
        }

        public VisitorEvent(EventType type, Guid visitorGuid, Dictionary<string, object> payload) : base(type)
        {
            this.VisitorGuid = visitorGuid;
            this.Payload = payload;
        }

        public Guid VisitorGuid { get; }

        public Dictionary<string, object> Payload { get; }
    }
}
