using Geolocation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public class VisitorDto
    {

        public Guid Guid { get; }

        public DateTime DateOfBirth { get; }

        public double Length { get; }

        public Coordinate CurrentLocation { get; set; }

        public VisitorDto() { }

        public VisitorDto(Guid guid, DateTime dateOfBirth, double length, Coordinate currentLocation)
        {
            this.Guid = guid;
            this.DateOfBirth = dateOfBirth;
            this.Length = length;
            this.CurrentLocation = currentLocation;
        }
    }
}
