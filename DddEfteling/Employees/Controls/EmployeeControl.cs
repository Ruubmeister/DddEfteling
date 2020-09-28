using DddEfteling.Park.Employees.Entities;
using DddEfteling.Shared.Boundary;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
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

        public static List<WorkplaceSkill> GetPossibleSkillsFromSkill(WorkplaceSkill skill) =>
            skill switch
            {
                WorkplaceSkill.Control => new List<WorkplaceSkill>() { WorkplaceSkill.Control, WorkplaceSkill.Host },
                WorkplaceSkill.Cook => new List<WorkplaceSkill>() { WorkplaceSkill.Cook },
                WorkplaceSkill.Engineer => new List<WorkplaceSkill>() { WorkplaceSkill.Engineer },
                WorkplaceSkill.Host => new List<WorkplaceSkill>() { WorkplaceSkill.Host, WorkplaceSkill.Sell },
                WorkplaceSkill.Sell => new List<WorkplaceSkill>() { WorkplaceSkill.Sell, WorkplaceSkill.Host },
                _ => throw new NotImplementedException()
            };

        public Employee HireEmployee(String firstName, String lastName, WorkplaceSkill skill)
        {
            List<WorkplaceSkill> skills = GetPossibleSkillsFromSkill(skill);

            Employee employee = new Employee(firstName, lastName, skills);
            Employees.Add(employee);

            logger.LogInformation($"Hired employee: {employee.FirstName} {employee.LastName}");
            return employee;
        }

        public void AssignEmployee(WorkplaceDto workplace, WorkplaceSkill skill)
        {
            Employee employee = Employees.DefaultIfEmpty(HireEmployee(nameService.RandomFirstName(), nameService.RandomLastName(), skill))
                .FirstOrDefault(employee => employee.ActiveWorkplace == null && employee.Skills.Contains(skill));

            employee.GoToWork(workplace, skill);

            logger.LogInformation($"Employee {employee.FirstName} {employee.LastName} assigned to workspace");
        }

        public Employee FindEmployeeByName(String firstName, String lastName)
        {
            return Employees.FirstOrDefault(employee => employee.FirstName == firstName && employee.LastName == lastName);
        }

        public List<Employee> GetEmployees(WorkplaceDto workplace)
        {
            return Employees.Where(employee => employee.ActiveWorkplace.Equals(workplace)).ToList();
        }
    }

    public interface IEmployeeControl
    {
        public Employee HireEmployee(String firstName, String lastName, WorkplaceSkill skill);
        public void AssignEmployee(WorkplaceDto workplace, WorkplaceSkill skill);
        public Employee FindEmployeeByName(String firstName, String lastName);

        public List<Employee> GetEmployees(WorkplaceDto workplace);
    }
}
