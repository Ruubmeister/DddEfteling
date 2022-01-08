using Geolocation;
using System;

namespace DddEfteling.Shared.Boundaries
{
    public class VisitorDto
    {

        public Guid Guid { get; set; }

        public DateTime DateOfBirth { get; set; }

        public double Length { get; set; }

        public Coordinate CurrentLocation { get; set; }

        public VisitorDto() { }

        public VisitorDto(Guid guid, DateTime dateOfBirth, double length, Coordinate currentLocation)
        {
            Guid = guid;
            DateOfBirth = dateOfBirth;
            Length = length;
            CurrentLocation = currentLocation;
        }
    }
}
