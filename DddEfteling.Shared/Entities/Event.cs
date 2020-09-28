using MediatR;
using System.Collections.Generic;

namespace DddEfteling.Shared.Entities
{
    public class Event : INotification
    {
        public EventSource Source { get; }
        public EventType Type { get; }
        public Dictionary<string, string> Payload { get; }
        public Event(EventType type, EventSource source, Dictionary<string, string> payload)
        {
            this.Type = type;
            this.Source = source;
            this.Payload = payload;
        }
    }
}
