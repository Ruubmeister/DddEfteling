using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Shared.Entities
{
    public class Event
    {
        public EventSource Source { get; }
        public EventType Type { get; }
        public Dictionary<string, string> Payload { get; }
        public Event(EventType type, EventSource source, Dictionary<string, string> payload)
        {
            Type = type;
            Source = source;
            Payload = payload;
        }
        
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || GetType() != obj.GetType())
            {
                return false;
            }
            else {
                var e = (Event) obj;

                return Type.Equals(e.Type) && Source.Equals(e.Source) && Payload.Count == e.Payload.Count && 
                                                 !Payload.Except(e.Payload).Any();
            }
        }
        
        public override int GetHashCode() => (Type, Source, Payload).GetHashCode();
    }
}
