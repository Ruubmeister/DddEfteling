using DddEfteling.Park.Employees.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Employees.Entities
{
    public class EmployeeTest
    {
        [Fact]
        public void Construct_employeeIsConstructed_expectsEmployee()
        {
            DateTime dateOfBirth = DateTime.Parse("24-11-1988");
            Employee employee = new Employee("Jan", "Jansen", new List<Skill>(){ Skill.Engineer });

            Assert.Equal("Jan", employee.FirstName);
            Assert.Equal("Jansen", employee.LastName);
            Assert.True(employee.Id != null);
            Assert.True(employee.ActiveWorkspace == null);
            Assert.True(employee.ActiveSkill == null);
            Assert.Equal(new List<Skill>() { Skill.Engineer }, employee.Skills);
        }
    }
}
