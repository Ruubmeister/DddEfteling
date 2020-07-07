using DddEfteling.Park.Entrances.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Entrances.Controls
{
    public class EntranceControl: IEntranceControl
    {
        private EntranceStatus status;
        private readonly IMediator mediator;
        private readonly ILogger<IEntranceControl> logger;
        
        public EntranceControl(IMediator mediator, ILogger<IEntranceControl> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async void OpenPark()
        {
            this.status = EntranceStatus.Open;
            logger.LogInformation("Park is opened");
            await mediator.Publish(new EntranceEvent(Common.Entities.EventType.StatusChanged));
        }

        public async void ClosePark()
        {
            this.status = EntranceStatus.Closed;
            logger.LogInformation("Park is closed");
            await mediator.Publish(new EntranceEvent(Common.Entities.EventType.StatusChanged));
        }

        public bool IsOpen()
        {
            return this.status.Equals(EntranceStatus.Open);
        }

        public Ticket SellTicket(TicketType type)
        {
            logger.LogDebug($"Ticket of type {type} sold");
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
