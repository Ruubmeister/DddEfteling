using DddEfteling.Park.Common.Control;
using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Employees.Controls
{
    public class EmployeeControl: IEmployeeControl
    {

        private HashSet<Employee> Employees { get; }

        private readonly INameService nameService;
        private readonly ILogger<EmployeeControl> logger;

        public EmployeeControl(INameService nameService, ILogger<EmployeeControl> logger)
        {
            Employees = new HashSet<Employee>();
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
        }

        public Employee FindEmployeeByName(String firstName, String lastName)
        {
            return Employees.FirstOrDefault(employee => employee.FirstName == firstName && employee.LastName == lastName);
        }
    }

    public interface IEmployeeControl
    {
        public Employee HireEmployee(String firstName, String lastName, Skill skill);
        public void AssignEmployee(Workspace workspace, Skill skill);
        public Employee FindEmployeeByName(String firstName, String lastName);
    }
}
