using System;

namespace DddEfteling.Park.Entities
{
    public class Ticket
    {
        public Ticket(TicketType type)
        {
            Type = type;
            Day = DateTime.Today;
        }
        public DateTime Day { get; }
        public TicketType Type { get; }

    }
}
