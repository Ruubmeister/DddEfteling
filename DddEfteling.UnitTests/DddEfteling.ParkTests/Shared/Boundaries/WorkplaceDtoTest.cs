using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Entities;
using System;
using Xunit;

namespace DddEfteling.ParkTests.Shared.Boundaries
{
    public class WorkplaceDtoTest
    {
        [Fact]
        public void Constructors_ConstructDto_ExpectDto()
        {
            Guid guid = Guid.NewGuid();
            WorkplaceDto workplaceDto = new WorkplaceDto(guid, LocationType.FAIRYTALE);

            Assert.Equal(LocationType.FAIRYTALE, workplaceDto.LocationType);
            Assert.Equal(guid, workplaceDto.Guid);
        }

        [Fact]
        public void Setters_ConstructAndUseSetters_ExpectDto()
        {
            Guid guid = Guid.NewGuid();
            WorkplaceDto workplaceDto = new WorkplaceDto();

            workplaceDto.Guid = guid;
            workplaceDto.LocationType = LocationType.RIDE;

            Assert.Equal(guid, workplaceDto.Guid);
            Assert.Equal(LocationType.RIDE, workplaceDto.LocationType);
        }
    }
}
