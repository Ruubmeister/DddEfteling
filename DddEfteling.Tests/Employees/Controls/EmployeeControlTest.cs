using DddEfteling.Park.Common.Control;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using Microsoft.Extensions.Logging;
using Moq;
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
            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger);

            Employee employee = employeeControl.HireEmployee("First", "Last", Skill.Control);

            Assert.NotNull(employee);
            Assert.Equal(employeeControl.FindEmployeeByName("First", "Last"), employee);
        }

        [Fact]
        public void FindEmployee_nonExistingEmployee_expectsNull()
        {
            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger);
            Assert.True(employeeControl.FindEmployeeByName("First", "Last") == null);
        }
    }
}
