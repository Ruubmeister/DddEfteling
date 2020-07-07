using DddEfteling.Park.Common.Entities;

namespace DddEfteling.Park.Entrances.Entities
{
    public class EntranceEvent : Event
    {
        public EntranceEvent(EventType type) : base(type) { }
    }
}
