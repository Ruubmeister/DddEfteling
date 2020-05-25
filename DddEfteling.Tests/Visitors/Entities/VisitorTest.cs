using DddEfteling.Visitors.Visitors.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Visitors.Entities
{
    public class VisitorTest
    {
        [Fact]
        public void Construct_createVisitor_expectVisitor()
        {
            DateTime dateOfBirth = DateTime.Now;
            dateOfBirth.Subtract(TimeSpan.FromDays(365*20));
            Visitor visitor = new Visitor(dateOfBirth, 1.73);

            Assert.Equal(dateOfBirth, visitor.DateOfBirth );
            Assert.False(visitor.Guid == Guid.Empty);
            Assert.Equal(1.73, visitor.Length);
        }
    }
}
