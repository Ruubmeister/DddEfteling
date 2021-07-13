using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Geolocation;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Visitors.Entities
{
    public class VisitorRepository
    {
        private readonly Random random;
        private readonly ILogger<VisitorRepository> logger;
        private readonly Coordinate startCoordinate = new Coordinate(51.649175, 5.045545);
        private ConcurrentBag<Visitor> Visitors { get; } = new ConcurrentBag<Visitor>();

        public VisitorRepository(Random random, ILogger<VisitorRepository> logger)
        {
            this.logger = logger;
            this.random = random;
        }

        public VisitorRepository(Random random, ILogger<VisitorRepository> logger, ConcurrentBag<Visitor> visitors)
        {
            this.logger = logger;
            this.random = random;
            this.Visitors = visitors;
        }
        
        public List<Visitor> All()
        {
            return Visitors.ToList();
        }
        
        public List<Visitor> AddVisitors(int number)
        {
            var result = new List<Visitor>();
            for (int i = 1; i <= number; i++)
            {
                logger.LogDebug($"Adding {i} visitors");
                // Todo: Fix hardcoded below
                Visitor visitor = new Visitor(DateTime.Now, 1.55, startCoordinate, random);
                Visitors.Add(visitor);
                result.Add(visitor);
            }

            return result;
        }
        
        public Visitor GetVisitor(Guid guid)
        {
            return this.Visitors.First(visitor => visitor.Guid.Equals(guid));
        }

        public List<Visitor> IdleVisitors()
        {
            DateTime now = DateTime.Now;
            return this.Visitors.Where(visitor => visitor.AvailableAt <= now).ToList();
        }

    }

    public interface IVisitorRepository
    {
        public List<Visitor> All();
        
        public List<Visitor> AddVisitors(int number);

        public Visitor GetVisitor(Guid guid);

        public List<Visitor> IdleVisitors();
    }
}