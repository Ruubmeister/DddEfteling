using DddEfteling.Shared.Entities;
using MediatR;
using System;
using System.Collections.Generic;

namespace DddEfteling.Visitors.Entities
{
    public class VisitorEvent : INotification
    {
        public VisitorEvent(EventType type, Guid visitorGuid)
        {
            Type = type;
            VisitorGuid = visitorGuid;
        }

        public VisitorEvent(EventType type, Guid visitorGuid, Dictionary<string, object> payload)
        {
            VisitorGuid = visitorGuid;
            Type = type;
            Payload = payload;
        }
        public EventType Type { get; }

        public Guid VisitorGuid { get; }

        public Dictionary<string, object> Payload { get; }
    }
}
