using System;

namespace DddEfteling.Visitors.Visitors.Entities
{
    public class Visitor
    {
        public Visitor(DateTime dateOfBirth, double length)
        {
            Guid = Guid.NewGuid();
            DateOfBirth = dateOfBirth;
            Length = length;
        }

        public Guid Guid { get; }

        public DateTime DateOfBirth { get; }

        public double Length { get; }
    }
}
