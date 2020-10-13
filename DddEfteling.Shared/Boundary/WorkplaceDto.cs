using DddEfteling.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public class WorkplaceDto
    {
        public Guid Guid { get; set; }
        public LocationType LocationType { get; set; }

        public WorkplaceDto(Guid guid, LocationType locationType)
        {
            this.Guid = guid;
            this.LocationType = LocationType;
        }
    }
}
