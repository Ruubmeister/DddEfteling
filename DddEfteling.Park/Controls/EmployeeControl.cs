using DddEfteling.Park.Boundaries;
using DddEfteling.Park.Entities;
using DddEfteling.Shared.Boundaries;
using DddEfteling.Shared.Controls;
using DddEfteling.Shared.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Controls
{
    public class EmployeeControl : IEmployeeControl
    {

        private ConcurrentBag<Employee> Employees { get; }

        private readonly INameService nameService;
        private readonly ILogger<EmployeeControl> logger;
        private readonly IEventProducer eventProducer;

        public EmployeeControl(INameService nameService, ILogger<EmployeeControl> logger, IEventProducer eventProducer)
        {
            Employees = new ConcurrentBag<Employee>();
            this.nameService = nameService;
            this.logger = logger;
            this.eventProducer = eventProducer;
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
            var skills = GetPossibleSkillsFromSkill(skill);

            var employee = new Employee(firstName, lastName, skills);
            Employees.Add(employee);

            logger.LogInformation($"Hired employee: {employee.FirstName} {employee.LastName}");
            return employee;
        }

        public void AssignEmployee(WorkplaceDto workplace, WorkplaceSkill skill)
        {
            var employee = Employees.DefaultIfEmpty(HireEmployee(nameService.RandomFirstName(), nameService.RandomLastName(), skill))
                .FirstOrDefault(employee => employee.ActiveWorkplace == null && employee.Skills.Contains(skill));

            employee.GoToWork(workplace, skill);

            var payload = new Dictionary<string, string>()
            {
                { "Employee", employee.Guid.ToString()},
                {"Workplace", JsonConvert.SerializeObject(workplace) },
                {"Skill", skill.ToString() }
            };
            var outgoingEvent = new Event(EventType.EmployeeChangedWorkplace, EventSource.Employee, payload);
            eventProducer.Produce(outgoingEvent);

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
