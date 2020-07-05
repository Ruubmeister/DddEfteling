using DddEfteling.Park.Entrances.Entities;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Entrances.Controls
{
    public class EntranceControl: IEntranceControl
    {
        private EntranceStatus status;
        private IMediator mediator;
        public EntranceControl(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async void OpenPark()
        {
            this.status = EntranceStatus.Open;
            await mediator.Publish(new EntranceEvent(Common.Entities.EventType.StatusChanged));
        }

        public async void ClosePark()
        {
            this.status = EntranceStatus.Closed;
            await mediator.Publish(new EntranceEvent(Common.Entities.EventType.StatusChanged));
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

    public interface IEntranceControl {

        void OpenPark();
        void ClosePark();

        bool IsOpen();
    }
}
