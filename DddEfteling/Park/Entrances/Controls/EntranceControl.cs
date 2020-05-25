using DddEfteling.Park.Entrances.Entities;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Entrances.Controls
{
    public class EntranceControl: IEntranceControl
    {
        private EntranceStatus status;

        public void openPark()
        {
            this.status = EntranceStatus.Open;
        }

        public void closePark()
        {
            this.status = EntranceStatus.Closed;
        }

        public bool IsOpen()
        {
            return this.status.Equals(EntranceStatus.Open);
        }

        public Ticket SellTicket(TicketType type)
        {
            return new Ticket(type);
        }

        public List<Ticket> SellTickets(List<TicketType> type)
        {
            return type.Select(localType => new Ticket(localType)).ToList();
        }
    }

    interface IEntranceControl { 
    }
}
