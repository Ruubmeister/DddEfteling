using System;

namespace DddEfteling.Park.Entrances.Entities
{
    public class Ticket
    {
        public Ticket(TicketType type)
        {
            this.Type = type;
            this.Day = DateTime.Today;
        }
        public DateTime Day { get; }
        public TicketType Type { get; }

    }
}
