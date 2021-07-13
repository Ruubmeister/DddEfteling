using System;
using System.Collections.Generic;
using System.Drawing;
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
            this.Type = type;
            this.Source = source;
            this.Payload = payload;
        }
        
        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                Event e = (Event) obj;

                return Type.Equals(e.Type) && Source.Equals(e.Source) && Payload.Count == e.Payload.Count && 
                                                 !Payload.Except(e.Payload).Any();
            }
        }
        
        public override int GetHashCode() => (Type, Source, Payload).GetHashCode();
    }
}
