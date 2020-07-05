using DddEfteling.Park.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.Visitors.Entities
{
    public class VisitorEvent : Event
    {
        public VisitorEvent(EventType type, Guid visitorGuid): base(type)
        {
            this.VisitorGuid = visitorGuid;
        }

        public Guid VisitorGuid { get; }
    }
}
