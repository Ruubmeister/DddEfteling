using DddEfteling.Park.Common.Entities;
using System;

namespace DddEfteling.Park.Employees.Entities
{
    public class Employee
    {

        public Employee(String firstName, String lastName, DateTime dateOfBirth) {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.Id = Guid.NewGuid();
            this.StartDate = DateTime.Now;
        }

        public Guid Id { get; }
        public String FirstName { get; }
        public String LastName { get; }

        public DateTime DateOfBirth { get; }
        public DateTime StartDate { get; }

        public Workspace CurrentWorkspace { get; set; }

        public void GoToWork(Workspace workspace)
        {
            workspace.EmployeeStartsShift(this);
        }

        public void StopWorking()
        {
            if (CurrentWorkspace != null)
            {
                CurrentWorkspace.EmployeeStopsShift(this);
            }
        }
    }
}
