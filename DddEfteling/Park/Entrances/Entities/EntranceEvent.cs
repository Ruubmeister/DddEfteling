using DddEfteling.Park.Common.Entities;
using MediatR;
using System;

namespace DddEfteling.Park.Entrances.Entities
{
    public class EntranceEvent : Event
    {
        public EntranceEvent(EventType type) : base(type) { }
    }
}
