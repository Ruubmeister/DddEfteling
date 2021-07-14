using System;
using System.Collections.Concurrent;
using System.Linq;
using DddEfteling.Visitors.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DddEfteling.VisitorTests.Entities
{
    public class VisitorRepositoryTest
    {
        private Random random = new Random();
        private Mock<ILogger<VisitorRepository>> loggerMock = new Mock<ILogger<VisitorRepository>>();
        
        [Fact]
        public void All_GivenHasVisitors_ExpectVisitors()
        {

            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor()
            };

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            
            Assert.Equal(visitors, visitorRepository.All());
        }
        
        [Fact]
        public void All_GivenNoVisitors_ExpectEmptyList()
        {
            VisitorRepository visitorRepository = new(random, loggerMock.Object);
            
            Assert.Empty( visitorRepository.All());
        }

        [Fact]
        public void AddVisitors_AddThreeVisitors_ExpectThreeVisitors()
        {
            VisitorRepository visitorRepository = new(random, loggerMock.Object);
            visitorRepository.AddVisitors(3);
            
            Assert.Equal(3, visitorRepository.All().Count);
        }

        [Fact]
        public void GetVisitor_HasVisitor_ExpectVisitor()
        {
            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor()
            };

            Visitor firstVisitor = visitors.First();

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            
            Assert.Equal(firstVisitor, visitorRepository.GetVisitor(firstVisitor.Guid));
        }
        
        [Fact]
        public void GetVisitor_DoesNotHaveVisitor_ExpectException()
        {
            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor()
            };

            Visitor missingVisitor = new Visitor();

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            Assert.Throws<InvalidOperationException>(() => visitorRepository.GetVisitor(missingVisitor.Guid));

        }

        [Fact]
        public void IdleVisitors_HasNoIdleVisitors_ExpectEmptyList()
        {
            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor()
            };

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            
            Assert.Empty(visitorRepository.IdleVisitors());
        }
        
        [Fact]
        public void IdleVisitors_HasAllIdleVisitors_ExpectAllIdleVisitors()
        {
            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor(),
                new Visitor(),
                new Visitor(),
                new Visitor()
            };
            
            foreach(Visitor visitor in visitors)
            {
                visitor.AvailableAt = DateTime.Now;
            }

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            
            Assert.Equal(5, visitorRepository.IdleVisitors().Count);
        }
        
        [Fact]
        public void IdleVisitors_HasSomeIdleVisitors_ExpectSomeIdleVisitors()
        {
            ConcurrentBag<Visitor> visitors = new ConcurrentBag<Visitor>()
            {
                new Visitor(),
                new Visitor(),
                new Visitor(),
                new Visitor(),
                new Visitor()
            };
            
            for( int i = 0; i < visitors.Count; i++)
            {
                if (i is 0 or 2 or 3)
                {
                    Visitor visitor = visitors.ElementAt(i);
                    visitor.AvailableAt = DateTime.Now;
                }
                
            }

            VisitorRepository visitorRepository = new(random, loggerMock.Object, visitors);
            
            Assert.Equal(3, visitorRepository.IdleVisitors().Count);
        }
    }
}