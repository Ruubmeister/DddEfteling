using DddEfteling.Park.Entrances.Controls;
using DddEfteling.Park.Entrances.Entities;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Entrances.Controls
{
    public class EntranceControlTest
    {
        [Fact]
        public void changeParkStatus_openAndClosePark_expectsStatusToBeChanged()
        {
            EntranceControl entranceControl = new EntranceControl();
            entranceControl.openPark();
            Assert.True(entranceControl.IsOpen());

            entranceControl.closePark();
            Assert.False(entranceControl.IsOpen());
        }

        [Fact]
        public void sellTicket_ticketForFamily_expectsTicket()
        {
            EntranceControl entranceControl = new EntranceControl();

            Ticket ticket = entranceControl.SellTicket(TicketType.Family);
            Assert.NotNull(ticket);
            Assert.Equal(TicketType.Family, ticket.Type);
            Assert.Equal(ticket.Day.Date, DateTime.Now.Date);
        }

        [Fact]
        public void sellTickets_differentTicketTypes_expectsTickets()
        {
            EntranceControl entranceControl = new EntranceControl();

            List<TicketType> ticketTypes = new List<TicketType>();
            ticketTypes.Add(TicketType.Adult);
            ticketTypes.Add(TicketType.Adult);
            ticketTypes.Add(TicketType.Seniors);
            ticketTypes.Add(TicketType.Child);
            ticketTypes.Add(TicketType.Child);
            ticketTypes.Add(TicketType.Child);

            List<Ticket> tickets = entranceControl.SellTickets(ticketTypes);

            Assert.NotNull(tickets);
            Assert.Equal(6, tickets.Count);
            Assert.Equal(2, tickets.FindAll(ticket => ticket.Type == TicketType.Adult).Count);
            Assert.Single(tickets.FindAll(ticket => ticket.Type == TicketType.Seniors));
            Assert.Equal(3, tickets.FindAll(ticket => ticket.Type == TicketType.Child).Count);
        }

    }

}
