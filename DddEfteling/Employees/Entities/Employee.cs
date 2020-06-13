using DddEfteling.Park.Common.Entities;
using System;
using System.Collections.Generic;

namespace DddEfteling.Park.Employees.Entities
{
    public class Employee
    {

        public Employee(String firstName, String lastName,  List<Skill> skills) {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Id = Guid.NewGuid();
            this.Skills = skills;
        }

        public Guid Id { get; }
        public String FirstName { get; }
        public String LastName { get; }

        public Workspace ActiveWorkspace { get; private set; }

        public List<Skill> Skills { get; }

        public Skill? ActiveSkill {  get; private set; }

        public void GoToWork(Workspace workspace, Skill skill)
        {
            this.ActiveWorkspace = workspace;
            this.ActiveSkill = skill;
        }

        public void StopWork()
        {
            if (this.ActiveWorkspace != null){
                this.ActiveWorkspace = null;
                this.ActiveSkill = null;

                //Also send event
            }
        }
    }
}
