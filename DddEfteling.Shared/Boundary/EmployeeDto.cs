using System;
using System.Collections.Generic;
using System.Text;

namespace DddEfteling.Shared.Boundary
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
            this.Guid = guid;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Skills = skills;
        }

    }
}
