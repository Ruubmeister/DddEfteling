using System;
using System.Collections.Generic;

namespace DddEfteling.Shared.Boundaries
{
    public class EmployeeDto
    {
        public Guid Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> Skills { get; set; }

        public EmployeeDto() { }

        public EmployeeDto(Guid guid, string firstName, string lastName, List<string> skills)
        {
            Guid = guid;
            FirstName = firstName;
            LastName = lastName;
            Skills = skills;
        }

    }
}
