using System;
using DddEfteling.Shared.Controls;
using DddEfteling.Visitors.Entities;

namespace DddEfteling.Visitors.Controls
{
    public class VisitorMovementService: IVisitorMovementService
    {
        private readonly Random random = new (); // Todo: Fix this random generator to injected one

        public static bool IsInLocationRange(Visitor visitor)
        {
            return CoordinateExtensions.IsInRange(visitor.CurrentLocation, visitor.TargetLocation.Coordinates,
                visitor.NextStepDistance);
        }
        
        public void SetNextStepDistance(Visitor visitor)
        {
            var normalizedStep = (double) random.Next(100, 300) / 100;
            var timeIdle = visitor.AvailableAt.HasValue ? DateTime.Now - visitor.AvailableAt.Value : 
                TimeSpan.FromSeconds(1);

            visitor.NextStepDistance = timeIdle.TotalSeconds * normalizedStep;
        }
    }

    public interface IVisitorMovementService
    {
        public void SetNextStepDistance(Visitor visitor);
    }
}