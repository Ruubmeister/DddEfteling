using MediatR;

namespace DddEfteling.Park.Common.Entities
{
    public abstract class Event : INotification
    {
        public EventType Type { get; }
        protected Event(EventType type)
        {
            this.Type = type;
        }
    }
}
