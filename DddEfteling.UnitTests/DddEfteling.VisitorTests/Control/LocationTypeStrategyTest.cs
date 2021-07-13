using DddEfteling.Shared.Entities;
using DddEfteling.Visitors.Controls;
using Moq;
using Xunit;

namespace DddEfteling.VisitorTests.Control
{
    public class LocationTypeStrategyTest
    {
        [Fact]
        public void RegisterAndGetStrategy_GivenNewStrategy_ExpectSuccessful()
        {
            LocationType type = LocationType.STAND;
            IVisitorLocationStrategy strategy = new Mock<IVisitorLocationStrategy>().Object;

            LocationTypeStrategy locationTypeStrategy = new ();
            locationTypeStrategy.Register(type, strategy);
            
            Assert.Equal(strategy, locationTypeStrategy.GetStrategy(type));
        }
        
        [Fact]
        public void RegisterAndGetStrategy_GivenExistingStrategy_ExpectSuccessful()
        {
            LocationType type = LocationType.STAND;
            IVisitorLocationStrategy strategy = new Mock<IVisitorLocationStrategy>().Object;

            LocationTypeStrategy locationTypeStrategy = new ();
            locationTypeStrategy.Register(type, strategy);
            
            Assert.Equal(strategy, locationTypeStrategy.GetStrategy(type));
            
            locationTypeStrategy.Register(type, strategy);
            Assert.Equal(strategy, locationTypeStrategy.GetStrategy(type));
        }
        
        [Fact]
        public void RegisterAndGetStrategy_GivenMissingStrategy_ExpectNull()
        {
            LocationType type = LocationType.STAND;
            IVisitorLocationStrategy strategy = new Mock<IVisitorLocationStrategy>().Object;

            LocationTypeStrategy locationTypeStrategy = new ();
            locationTypeStrategy.Register(type, strategy);

            Assert.Null(locationTypeStrategy.GetStrategy(LocationType.RIDE));
        }

    }
}