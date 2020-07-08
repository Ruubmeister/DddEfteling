using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Rides.Entities;
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

        [Fact]

        public void GoToWorkAndStop_GoToRideAndStop_ExpectRide()
        {
            Ride ride = new Ride();
            Employee employee = new Employee("Jan", "Jansen", new List<Skill>() { Skill.Engineer });

            employee.GoToWork(ride, Skill.Engineer);
            Assert.Equal(Skill.Engineer, employee.ActiveSkill );
            Assert.Equal(ride, employee.ActiveWorkspace);

            employee.StopWork();
            Assert.Null(employee.ActiveSkill);
            Assert.Null(employee.ActiveWorkspace);

        }
    }
}
