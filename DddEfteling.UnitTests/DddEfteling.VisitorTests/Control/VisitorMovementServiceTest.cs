using DddEfteling.Shared.Boundaries;
using DddEfteling.Visitors.Controls;
using DddEfteling.Visitors.Entities;
using Geolocation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DddEfteling.VisitorTests.Control
{
    public class VisitorMovementServiceTest
    {
        private VisitorMovementService visitorMovementService = new();
        
        
        [Fact]
        public void SetNextStep_GivenVisitorWithNoNextStep_ExpectNextStep()
        {
            Visitor visitor = new Visitor();
            Assert.Equal(0.0, visitor.NextStepDistance);
            
            visitorMovementService.SetNextStepDistance(visitor);
            Assert.True(visitor.NextStepDistance > 0.0);
        }
        
        [Fact]
        public void SetNextStep_GivenVisitorWithNextStep_ExpectNextStepUpdated()
        {
            Visitor visitor = new Visitor();
            visitor.NextStepDistance = 10.1;
            
            visitorMovementService.SetNextStepDistance(visitor);
            Assert.NotEqual(10.1, visitor.NextStepDistance);
        }

        [Fact]

        public void IsInLocationRange_LocationIsInRange_ExpectTrue()
        {
            Visitor visitor = new Visitor();
            visitor.CurrentLocation = new Coordinate(51.6491558,5.0455948);
            RideDto rideDto = new RideDto();
            rideDto.Coordinates = new Coordinate(51.6491538, 5.0456268);
            visitor.TargetLocation = rideDto;
            visitor.NextStepDistance = 10.0;
            
            Assert.True(VisitorMovementService.IsInLocationRange(visitor));
        }
        
        [Fact]
        public void IsInLocationRange_LocationNotInRange_ExpectFalse()
        {
            Visitor visitor = new Visitor();
            visitor.CurrentLocation = new Coordinate(51.6491558,5.0455948);
            RideDto rideDto = new RideDto();
            rideDto.Coordinates = new Coordinate(51.6491538, 5.0456268);
            visitor.TargetLocation = rideDto;
            visitor.NextStepDistance = 1.0;
            
            Assert.False(VisitorMovementService.IsInLocationRange(visitor));
        }
    }
}