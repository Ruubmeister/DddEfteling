using DddEfteling.Park.Common.Entities;
using DddEfteling.Park.Employees.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DddEfteling.Park.Employees.Controls
{
    public class EmployeeControl
    {

        private HashSet<Employee> Employees { get; }

        public EmployeeControl()
        {
            Employees = new HashSet<Employee>();
        }

        public Employee HireEmployee(String firstName, String lastName, DateTime dateOfBirth)
        {
            Employee employee = new Employee(firstName, lastName, dateOfBirth);
            Employees.Add(employee);
            return employee;
        }

        public void AssignEmployee(Workspace workspace)
        {
            if(!Employees.Any(employee => employee.CurrentWorkspace == null))
            {
                HireEmployee("Employee", (Employees.Count + 1).ToString(), DateTime.Now);
            }

            Employees.First(employee => employee.CurrentWorkspace == null).GoToWork(workspace);
        }

        public Employee FindEmployeeByName(String firstName, String lastName)
        {
            return Employees.FirstOrDefault(employee => employee.FirstName == firstName && employee.LastName == lastName);
        }
    }
}
