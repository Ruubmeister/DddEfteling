using System;
using DddEfteling.Shared.Controls;
using DddEfteling.Visitors.Entities;
using Microsoft.Extensions.Logging;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorMovementService: IVisitorMovementService
    {
        private ILogger<VisitorMovementService> logger;
        private readonly Random random = new Random(); // Todo: Fix this random generator to injected one

        public VisitorMovementService(ILogger<VisitorMovementService> logger)
        {
            this.logger = logger;
        }

        public static bool IsInLocationRange(Visitor visitor)
        {
            return CoordinateExtensions.IsInRange(visitor.CurrentLocation, visitor.TargetLocation.Coordinates,
                visitor.NextStepDistance);
        }
        
        public void SetNextStepDistance(Visitor visitor)
        {
            double normalizedStep = (double) random.Next(50, 150) / 100;
            TimeSpan timeIdle = visitor.AvailableAt.HasValue ? DateTime.Now - visitor.AvailableAt.Value : 
                TimeSpan.FromSeconds(1);

            visitor.NextStepDistance = timeIdle.TotalSeconds * normalizedStep;
        }
    }

    public interface IVisitorMovementService
    {
        public void SetNextStepDistance(Visitor visitor);
    }
}