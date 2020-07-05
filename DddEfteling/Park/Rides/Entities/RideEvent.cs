using DddEfteling.Park.Common.Entities;
using System;

namespace DddEfteling.Park.Rides.Entities
{
    public class RideEvent : Event
    {
        public RideEvent(EventType type, String rideName, object attachment = null) : base(type)
        {
            this.RideName = rideName;
            this.Attachment = attachment;
        }

        public String RideName { get; }
        public object Attachment { get; }
    }
}
