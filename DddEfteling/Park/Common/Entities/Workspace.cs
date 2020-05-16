using DddEfteling.Park.Employees.Entities;
using DddEfteling.Park.Realms.Entities;
using System.Collections.Generic;

namespace DddEfteling.Park.Common.Entities
{
    public abstract class Workspace
    {
        public Realm Realm { get; set; }
        public HashSet<Employee> Employees { get; set; }

        public void EmployeeStartsShift(Employee employee)
        {
            this.Employees.Add(employee);
        }

        public void EmployeeStopsShift(Employee employee)
        {
            this.Employees.Remove(employee);
        }
    }
}
