using DddEfteling.Shared.Entities;
using System;

namespace DddEfteling.Shared.Boundaries
{
    public class WorkplaceDto
    {
        public Guid Guid { get; set; }
        public LocationType LocationType { get; set; }

        public WorkplaceDto() { }

        public WorkplaceDto(Guid guid, LocationType locationType)
        {
            this.Guid = guid;
            this.LocationType = locationType;
        }
    }
}
