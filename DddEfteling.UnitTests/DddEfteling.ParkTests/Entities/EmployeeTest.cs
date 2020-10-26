using DddEfteling.Park.Entities;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Entities;
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
            Employee employee = new Employee("Jan", "Jansen", new List<WorkplaceSkill>(){ WorkplaceSkill.Engineer });

            Assert.Equal("Jan", employee.FirstName);
            Assert.Equal("Jansen", employee.LastName);
            Assert.True(employee.Guid != null);
            Assert.True(employee.ActiveWorkplace == null);
            Assert.True(employee.ActiveSkill == null);
            Assert.Equal(new List<WorkplaceSkill>() { WorkplaceSkill.Engineer }, employee.Skills);
        }

        [Fact]

        public void GoToWorkAndStop_GoToRideAndStop_ExpectRide()
        {
            WorkplaceDto ride = new WorkplaceDto(Guid.NewGuid(), LocationType.RIDE);
            Employee employee = new Employee("Jan", "Jansen", new List<WorkplaceSkill>() { WorkplaceSkill.Engineer });

            employee.GoToWork(ride, WorkplaceSkill.Engineer);
            Assert.Equal(WorkplaceSkill.Engineer, employee.ActiveSkill );
            Assert.Equal(ride, employee.ActiveWorkplace);

            employee.StopWork();
            Assert.Null(employee.ActiveSkill);
            Assert.Null(employee.ActiveWorkplace);

        }
    }
}
