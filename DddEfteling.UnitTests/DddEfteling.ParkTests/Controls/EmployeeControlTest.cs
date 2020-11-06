using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Controls;
using DddEfteling.Park.Entities;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace DddEfteling.ParkTests.Controls
{
    public class EmployeeControlTest
    {
        [Fact]
        public void HireEmployee_providesData_expectsNewEmployee()
        {
            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger, eventProducer);

            Employee employee = employeeControl.HireEmployee("First", "Last", WorkplaceSkill.Control);

            Assert.NotNull(employee);
            Assert.Equal(employeeControl.FindEmployeeByName("First", "Last"), employee);
        }

        [Fact]
        public void FindEmployee_nonExistingEmployee_expectsNull()
        {
            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger, eventProducer);
            Assert.True(employeeControl.FindEmployeeByName("First", "Last") == null);
        }

        [Fact]
        public void GetEmployee_givenRide_ExpectEmployee()
        {

            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger, eventProducer);

            Employee employee = employeeControl.HireEmployee("First", "Last", WorkplaceSkill.Control);
            WorkplaceDto ride = new WorkplaceDto(Guid.NewGuid(), LocationType.RIDE);

            employee.GoToWork(ride, WorkplaceSkill.Engineer);

            List<Employee> result = employeeControl.GetEmployees(ride);

            Assert.Contains(employee, result);
        }

        [Fact]
        public void AssignEmployee_givenRideAndSkill_ExpectEmployeeAssigned()
        {

            ILogger<EmployeeControl> logger = Mock.Of<ILogger<EmployeeControl>>();
            INameService nameService = new NameService();
            IEventProducer eventProducer = Mock.Of<IEventProducer>();
            EmployeeControl employeeControl = new EmployeeControl(nameService, logger, eventProducer);

            WorkplaceDto ride = new WorkplaceDto(Guid.NewGuid(), LocationType.RIDE);

            employeeControl.AssignEmployee(ride, WorkplaceSkill.Control);

            List<Employee> result = employeeControl.GetEmployees(ride);

            Assert.NotEmpty(result);
            Assert.Equal(ride, result[0].ActiveWorkplace);
            Assert.Equal(WorkplaceSkill.Control, result[0].ActiveSkill);
        }

        [Fact]
        public void GetPossibleSkillsFromSkill_GetGivenSkills_ExpectCorrectSkills()
        {
            List<WorkplaceSkill> skills = EmployeeControl.GetPossibleSkillsFromSkill(WorkplaceSkill.Control);

            Assert.Equal(2, skills.Count);
            Assert.Contains(WorkplaceSkill.Control, skills);
            Assert.Contains(WorkplaceSkill.Host, skills);

            skills = EmployeeControl.GetPossibleSkillsFromSkill(WorkplaceSkill.Cook);

            Assert.Single(skills);
            Assert.Contains(WorkplaceSkill.Cook, skills);

            skills = EmployeeControl.GetPossibleSkillsFromSkill(WorkplaceSkill.Engineer);

            Assert.Single(skills);
            Assert.Contains(WorkplaceSkill.Engineer, skills);

            skills = EmployeeControl.GetPossibleSkillsFromSkill(WorkplaceSkill.Host);

            Assert.Equal(2, skills.Count);
            Assert.Contains(WorkplaceSkill.Sell, skills);
            Assert.Contains(WorkplaceSkill.Host, skills);

            skills = EmployeeControl.GetPossibleSkillsFromSkill(WorkplaceSkill.Sell);

            Assert.Equal(2, skills.Count);
            Assert.Contains(WorkplaceSkill.Sell, skills);
            Assert.Contains(WorkplaceSkill.Host, skills);

        }
    }
}
