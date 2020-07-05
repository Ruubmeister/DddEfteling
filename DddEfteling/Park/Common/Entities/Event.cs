using MediatR;
using Microsoft.CodeAnalysis.CodeStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DddEfteling.Park.Common.Entities
{
    public abstract class Event : INotification
    {
        public EventType Type { get; }
        public Event(EventType type)
        {
            this.Type = type;
        }
    }
}
