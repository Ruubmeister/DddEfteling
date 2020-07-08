using DddEfteling.Park.Common.Control;
using DddEfteling.Park.Employees.Controls;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Rides.Entities;
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

        [Fact]
        public void GetEmployee_givenRide_ExpectEmployee()
        {

            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger);

            Employee employee = employeeControl.HireEmployee("First", "Last", Skill.Control);
            Ride ride = new Ride();

            employee.GoToWork(ride, Skill.Engineer);

            List<Employee> result = employeeControl.GetEmployees(ride);

            Assert.Contains(employee, result);
        }

        [Fact]
        public void AssignEmployee_givenRideAndSkill_ExpectEmployeeAssigned()
        {

            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger);

            Ride ride = new Ride();

            employeeControl.AssignEmployee(ride, Skill.Control);

            List<Employee> result = employeeControl.GetEmployees(ride);

            Assert.NotEmpty(result);
            Assert.Equal(ride, result[0].ActiveWorkspace);
            Assert.Equal(Skill.Control, result[0].ActiveSkill);
        }
    }
}
