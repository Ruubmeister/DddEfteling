using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
{
    public class EmployeeDto
    {
        public Guid Guid { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public List<string> Skills { get; }

        public EmployeeDto(Guid guid, string firstName, string lastName, List<string> skills)
        {
            this.Guid = guid;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Skills = skills;
        }

    }
}
