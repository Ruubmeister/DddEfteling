using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Entities;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Controls
{
    public class EntranceControl : IEntranceControl
    {
        private EntranceStatus status;
        private readonly ILogger<IEntranceControl> logger;
        private readonly IEventProducer eventProducer;

        public EntranceControl(ILogger<IEntranceControl> logger, IEventProducer eventProducer)
        {
            this.logger = logger;
            this.eventProducer = eventProducer;
        }

        public void OpenPark()
        {
            status = EntranceStatus.Open;
            logger.LogInformation("Park has opened");
            var eventOut = new Event(EventType.StatusChanged, EventSource.Park, new Dictionary<string, string>() { { "Status", "Open" } });
            eventProducer.Produce(eventOut);
        }

        public void ClosePark()
        {
            status = EntranceStatus.Closed;
            logger.LogInformation("Park has closed");
            var eventOut = new Event(EventType.StatusChanged, EventSource.Park, new Dictionary<string, string>() { { "Status", "Closed" } });
            eventProducer.Produce(eventOut);
        }

        public bool IsOpen()
        {
            return status.Equals(EntranceStatus.Open);
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

    public interface IEntranceControl
    {

        void OpenPark();
        void ClosePark();

        bool IsOpen();
    }
}
