using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace DddEfteling.Tests.Park.Employees.Controls
{
    public class EmployeeControlTest
    {
        [Fact]
        public void HireEmployee_providesData_expectsNewEmployee()
        {
            EmployeeControl employeeControl = new EmployeeControl();

            Employee employee = employeeControl.HireEmployee("First", "Last", DateTime.Now);

            Assert.NotNull(employee);
            Assert.Equal(employeeControl.FindEmployeeByName("First", "Last"), employee);
        }

        [Fact]
        public void FindEmployee_nonExistingEmployee_expectsNull()
        {
            EmployeeControl employeeControl = new EmployeeControl();
            Assert.True(employeeControl.FindEmployeeByName("First", "Last") == null);
        }
    }
}
