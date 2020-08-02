using DddEfteling.Park.Common.Control;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Rides.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Employees.Controls
{
    public class EmployeeControl: IEmployeeControl
    {

        private ConcurrentBag<Employee> Employees { get; }

        private readonly INameService nameService;
        private readonly ILogger<EmployeeControl> logger;

        public EmployeeControl(INameService nameService, ILogger<EmployeeControl> logger)
        {
            Employees = new ConcurrentBag<Employee>();
            this.nameService = nameService;
            this.logger = logger;
        }

        public static List<Skill> GetPossibleSkillsFromSkill(Skill skill) =>
            skill switch
            {
                Skill.Control => new List<Skill>() { Skill.Control, Skill.Host },
                Skill.Cook => new List<Skill>() { Skill.Cook },
                Skill.Engineer => new List<Skill>() { Skill.Engineer },
                Skill.Host => new List<Skill>() { Skill.Host, Skill.Sell },
                Skill.Sell => new List<Skill>() { Skill.Sell, Skill.Host },
                _ => throw new NotImplementedException()
            };

        public Employee HireEmployee(String firstName, String lastName, Skill skill)
        {
            List<Skill> skills = GetPossibleSkillsFromSkill(skill);

            Employee employee = new Employee(firstName, lastName, skills);
            Employees.Add(employee);

            logger.LogInformation($"Hired employee: {employee.FirstName} {employee.LastName}");
            return employee;
        }

        public void AssignEmployee(Workspace workspace, Skill skill)
        {
            Employee employee = Employees.DefaultIfEmpty(HireEmployee(nameService.RandomFirstName(), nameService.RandomLastName(), skill))
                .FirstOrDefault(employee => employee.ActiveWorkspace == null && employee.Skills.Contains(skill));

            employee.GoToWork(workspace, skill);

            logger.LogInformation($"Employee {employee.FirstName} {employee.LastName} assigned to workspace");
        }

        public Employee FindEmployeeByName(String firstName, String lastName)
        {
            return Employees.FirstOrDefault(employee => employee.FirstName == firstName && employee.LastName == lastName);
        }

        public List<Employee> GetEmployees(Ride ride)
        {
            return Employees.Where(employee => employee.ActiveWorkspace.Equals(ride)).ToList();
        }
    }

    public interface IEmployeeControl
    {
        public Employee HireEmployee(String firstName, String lastName, Skill skill);
        public void AssignEmployee(Workspace workspace, Skill skill);
        public Employee FindEmployeeByName(String firstName, String lastName);

        public List<Employee> GetEmployees(Ride ride);
    }
}
