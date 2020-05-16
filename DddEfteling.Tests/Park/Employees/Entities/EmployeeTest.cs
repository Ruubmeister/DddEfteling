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
            Employee employee = new Employee("Jan", "Jansen", dateOfBirth);

            Assert.Equal("Jan", employee.FirstName);
            Assert.Equal("Jansen", employee.LastName);
            Assert.Equal(dateOfBirth, employee.DateOfBirth);
            Assert.True(employee.StartDate < DateTime.Now);
            Assert.True(employee.Id != null);
            Assert.True(employee.CurrentWorkspace == null);
        }
    }
}
