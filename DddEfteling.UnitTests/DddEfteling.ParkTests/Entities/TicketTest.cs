using DddEfteling.Park.Entities;
using System;
using Xunit;

namespace DddEfteling.Tests.Park.Entrances.Entities
{
    public class TicketTest
    {
        [Fact]
        public void construct_createTicket_expectTicket()
        {
            Ticket ticket = new Ticket(TicketType.Child);
            Assert.Equal(TicketType.Child, ticket.Type);
            Assert.True(DateTime.Now > ticket.Day);
            Assert.Equal(DateTime.Today, ticket.Day.Date);
        }
        
    }
}
